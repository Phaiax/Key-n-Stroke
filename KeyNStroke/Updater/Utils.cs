using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Windows;
using System.Xml;

namespace KeyNStroke.Updater
{
    class Utils
    {
        #region Exceptions

        /// <summary>
        /// Exception thrown if the verification of the manifest or update.exe failed.
        /// </summary>
        public class VerificationException : Exception
        {
            public VerificationException()
            {
            }

            public VerificationException(string message)
                : base(message)
            {
            }

            public VerificationException(string message, Exception inner)
                : base(message, inner)
            {
            }
        }

        /// <summary>
        /// Exception thrown if the download of the manifest or update.exe failed.
        /// </summary>
        public class DownloadFailedException : Exception
        {
            public DownloadFailedException()
            {
            }

            public DownloadFailedException(string message)
                : base(message)
            {
            }

            public DownloadFailedException(string message, Exception inner)
                : base(message, inner)
            {
            }
        }

        #endregion

        #region Hash

        /// <summary>
        /// Calculates the SHA256 hash of the given file.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>The hash as string.</returns>
        public static string SHA256OfFile(string path)
        {
            SHA256 Sha256 = SHA256.Create();

            using (FileStream stream = File.OpenRead(path))
            {
                byte[] bytes = Sha256.ComputeHash(stream);
                string result = "";
                foreach (byte b in bytes) result += b.ToString("x2");
                return result;
            }
        }

        /// <summary>
        /// Calculates the SHA256 hash of the given bytes.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>The hash as string.</returns>
        public static string SHA256OfBytes(byte[] data)
        {
            SHA256 Sha256 = SHA256.Create();

            byte[] bytes = Sha256.ComputeHash(data);
            string result = "";
            foreach (byte b in bytes) result += b.ToString("x2");
            return result;
        }

        #endregion

        #region Permissions

        public static bool IsDirectoryWritable(string path)
        {
            try
            {
                string random = Path.Combine(path, Path.GetRandomFileName());
                using (FileStream fs = File.Create(random, 1, FileOptions.DeleteOnClose))
                { }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool HasAdminPrivileges()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        #endregion

        #region XML signature

        // https://docs.microsoft.com/de-de/dotnet/standard/security/how-to-sign-xml-documents-with-digital-signatures
        /// <summary>
        /// Sign an XML file with an RSA key.
        /// The signature can be verified with the other key of the key pair by using the function <code>VerifyXml()</code>.
        /// The singature is appended as an additional node to `xmlDoc`.
        /// </summary>
        /// <param name="xmlDoc">The document to be signed</param>
        /// <param name="rsaKey">The public or private key for signing.</param>
        public static void SignXml(XmlDocument xmlDoc, RSA rsaKey)
        {
            // Check arguments.
            if (xmlDoc == null)
                throw new ArgumentNullException(nameof(xmlDoc));

            // Create a SignedXml object.
            SignedXml signedXml = new SignedXml(xmlDoc)
            {
                // Add the key to the SignedXml document.
                SigningKey = rsaKey ?? throw new ArgumentNullException(nameof(rsaKey))
            };

            // Create a reference to be signed.
            Reference reference = new Reference
            {
                Uri = ""
            };

            // Add an enveloped transformation to the reference.
            XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform();
            reference.AddTransform(env);

            // Add the reference to the SignedXml object.
            signedXml.AddReference(reference);

            // Compute the signature.
            signedXml.ComputeSignature();

            // Get the XML representation of the signature and save
            // it to an XmlElement object.
            XmlElement xmlDigitalSignature = signedXml.GetXml();

            // Append the element to the XML document.
            xmlDoc.DocumentElement.AppendChild(xmlDoc.ImportNode(xmlDigitalSignature, true));
        }

        // https://docs.microsoft.com/de-de/dotnet/standard/security/how-to-sign-xml-documents-with-digital-signatures
        /// <summary>
        /// Verify the signature of an XML document signed by the function <code>SignXml()</code>.
        /// Throws an exception if the signature is not valid.
        /// </summary>
        /// <param name="xmlDoc">The signed document.</param>
        /// <param name="rsaKey">The public or private key (other one) that has been used for signing.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Wrong arguments</exception>
        /// <exception cref="CryptographicException">Verification failed</exception>
        public static void VerifyXml(XmlDocument xmlDoc, RSA rsaKey)
        {
            // Check arguments.
            if (xmlDoc == null)
                throw new ArgumentNullException("xmlDoc");
            if (rsaKey == null)
                throw new ArgumentNullException("pubKey");

            // Create a new SignedXml object and pass it
            // the XML document class.
            SignedXml signedXml = new SignedXml(xmlDoc);

            // Find the "Signature" node and create a new
            // XmlNodeList object.
            XmlNodeList nodeList = xmlDoc.GetElementsByTagName("Signature");

            // Throw an exception if no signature was found.
            if (nodeList.Count <= 0)
            {
                throw new CryptographicException("No Signature was found in the document.");
            }

            // This example only supports one signature for
            // the entire XML document.  Throw an exception
            // if more than one signature was found.
            if (nodeList.Count > 1)
            {
                throw new CryptographicException("More that one signature was found in the document.");
            }

            // Load the first <signature> node.
            signedXml.LoadXml((XmlElement)nodeList[0]);

            // Check the signature and return the result.
            bool valid = signedXml.CheckSignature(rsaKey);

            if (!valid)
            {
                throw new CryptographicException("Signature invalid.");
            }
        }

        #endregion

        #region Download Helper

        public delegate void ProgressCallBack(int progress);

        /// <summary>Downloads a ressource from the web. Provides progress information</summary>
        /// <returns>The downloaded bytes (never null)</returns>
        /// <exception cref="DownloadFailedException">Thrown if the download fails.</exception>
        private static byte[] DownloadWithTimeoutAndProgress(string url, ProgressCallBack progressCallBack, TimeSpan? timeout)
        {
            Exception exception = null; // probably a WebException
            byte[] data = null;
            bool canceled = false;

            using (WebClient wc = new WebClient())
            {
                wc.DownloadProgressChanged += (o, e) =>
                {
                    bool _canceled;
                    lock (e.UserState)
                    {
                        _canceled = canceled;
                    }
                    if (!canceled)
                    {
                        progressCallBack?.Invoke(e.ProgressPercentage);
                    }
                };
                wc.DownloadDataCompleted += (o, e) =>
                {
                    if (e.Error != null)
                    {
                        exception = e.Error;
                    }
                    else
                    {
                        data = e.Result;
                    }
                    lock (e.UserState)
                    {
                        //releases blocked thread
                        Monitor.Pulse(e.UserState);
                    }
                };

                var syncObject = new Object();

                lock (syncObject)
                {
                    SynchronizationContext orig = SynchronizationContext.Current;
                    // Use thread pool and not the SyncContext for WPF
                    //SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

                    wc.DownloadDataAsync(new Uri(url), syncObject);

                    SynchronizationContext.SetSynchronizationContext(orig);

                    if (timeout is TimeSpan to)
                    {
                        if (!Monitor.Wait(syncObject, to))
                        {
                            wc.CancelAsync();
                            throw new DownloadFailedException(exception.Message, exception);
                        }
                    }
                    else
                    {
                        Monitor.Wait(syncObject);
                    }
                }
            }

            if (exception != null)
            {
                throw new DownloadFailedException(exception.Message, exception);
            }
            if (data == null)
            {
                throw new DownloadFailedException("No data received.");
            }
            return data;
        }

        #endregion

        #region Download Update.exe

        /// <summary>
        /// Downloads the updated executable from the manifest and verifies its hash against the manifest.
        /// Throws an exception on download failure or verification failure.
        /// </summary>
        /// <param name="manifest">The verified manifest that specifies the hash and download url.</param>
        /// <param name="progressCallBack">Called on download progress change. This call originates from a different thread but only as long as this function blocks.</param>
        /// <returns>The downloaded data.</returns>
        /// <exception cref="DownloadFailedException">Thrown if the download fails.</exception>
        /// <exception cref="VerificationException">Thrown if the verification fails.</exception>
        public static byte[] DownloadExecutableAndVerifyHash(XmlDocument manifest, ProgressCallBack progressCallBack, TimeSpan? timeout)
        {
            var executableUrl = manifest.SelectSingleNode("manifest/update/download").InnerText;
            byte[] data = DownloadWithTimeoutAndProgress(executableUrl, progressCallBack, timeout);

            var expectedHash = manifest.SelectSingleNode("manifest/update/sha256").InnerText;
            var hash = SHA256OfBytes(data);

            if (hash != expectedHash)
            {
                throw new VerificationException("Update hash mismatch.");
            }
            else
            {
                return data;
            }
        }

        #endregion

        #region Download Manifest

        public static string ManifestUrl
        {
            get { return @"https://invisibletower.de/key-n-stroke/updateManifest.xml"; }
        }

        /// <summary>
        /// For debugging: Remember the latest downloaded manifest even if verification failed
        /// </summary>
        private static string _rawManifest;

        /// <summary>
        /// Download and verify the update manifest.
        /// Throws VerificationException if the manifest can't be validated.
        /// </summary>
        /// <returns>Returns the verified manifest.</returns>
        /// <exception cref="DownloadFailedException">Thrown if the download failed.</exception>
        /// <exception cref="VerificationException">Thrown if the signature could not be verified.</exception>
        public static XmlDocument DownloadAndVerifyManifest()
        {
            XmlDocument xmlDoc = new XmlDocument
            {
                PreserveWhitespace = true
            };
            TimeSpan oneMinute = new TimeSpan(0, 0, 3);
            
            byte[] xml = DownloadWithTimeoutAndProgress(ManifestUrl, null, oneMinute);
            
            try
            {
                _rawManifest = Encoding.UTF8.GetString(xml); // store for debug
            }
            catch (Exception e)
            {
                throw new VerificationException("Manifest invalid (no UTF-8)", e);
            }

            try
            {
                xmlDoc.LoadXml(_rawManifest);
            }
            catch (XmlException e)
            {
                throw new VerificationException("Manifest invalid (no xml)", e);
            }

            try
            {
                VerifyXml(xmlDoc, GetEmbeddedPubKey());
            }
            catch (CryptographicException e)
            {
                throw new VerificationException(e.Message);
            }

            return xmlDoc;
        }

        #endregion

        #region GetEmbeddedPubKey

        /// <summary>
        /// Return the embedded PubKey
        /// </summary>
        /// <returns></returns>
        public static RSA GetEmbeddedPubKey()
        {
            Stream pubKeyStream = Application.GetResourceStream(new Uri("pack://application:,,,/KeyNStroke;component/Resources/updateKey.pub.xml")).Stream;
            RSA pub = RSA.Create();
            using (StreamReader sr = new StreamReader(pubKeyStream))
            {
                pub.FromXmlString(sr.ReadToEnd());
            }
            return pub;
        }

        #endregion

        #region Version compare (CanUpdate())

        /// <summary>
        /// Checks if the updatable version is greater than the installed version.
        /// </summary>
        /// <returns></returns>
        public static bool CanUpdate(XmlDocument manifest)
        {
            try
            {
                string update_version = manifest.SelectSingleNode("manifest/update/version").InnerText;
                Version current = Assembly.GetExecutingAssembly().GetName().Version;
                Version update = Version.Parse(update_version);
                return (update.CompareTo(current) > 0);
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion
    }
}
