using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Xml;

namespace KeyNStroke.Updater
{
    class Admininstration
    {
        // Files for key creation and testing (all in current working directory)
        const string PATH_PUB_KEY = "updateKey.pub.xml";
        const string PATH_PRIV_KEY = "updateKey.priv.xml";
        const string PATH_UPDATE_MANIFEST = "updateManifest.xml";

        /// <summary>
        /// Checks the command line and executes administrative functions if the parameter is given.
        /// Returns true if the program should exit.
        /// </summary>
        /// <param name="args"></param>
        static public bool HandleArgs(string[] args)
        {
            if (args.Length > 0)
            {
                if (args[0] == "--create-signing-keypair")
                {
                    CreateSigningKeyPair();
                    return true;
                }
                if (args[0] == "--create-update-manifest")
                {
                    CreateUpdateManifest();
                    return true;
                }
                if (args[0] == "--sign-update-manifest")
                {
                    SignUpdateManifest();
                    return true;
                }
                if (args[0] == "--test-update")
                {
                    DownloadAndVerifyManifestAndUpdate();
                    return true;
                }
                if (args[0] == "--test-signing")
                {
                    TestSigning();
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Creates a new RSA key and saves it next to the executable.
        /// Put the public key into the Resources folder and bake it into the executable.
        /// Store the private key somewhere and never give it away.
        /// </summary>
        static void CreateSigningKeyPair()
        {

            RSA rsaKey = RSA.Create();
            string pub = rsaKey.ToXmlString(false);
            string priv = rsaKey.ToXmlString(true);

            using (StreamWriter outputFile = new StreamWriter(PATH_PUB_KEY))
            {
                outputFile.Write(pub);
            }

            using (StreamWriter outputFile = new StreamWriter(PATH_PRIV_KEY))
            {
                outputFile.Write(priv);
            }
        }

        /// <summary>
        /// Load the private key from the current directory.
        /// </summary>
        /// <returns>The private key.</returns>
        static RSA GetPrivateKey()
        {
            RSA priv = RSA.Create();
            using (var sr = new StreamReader(PATH_PRIV_KEY))
            {
                priv.FromXmlString(sr.ReadToEnd());
            }
            return priv;
        }

        /// <summary>
        /// Verifies that the embedded public key can be used to verify documents 
        /// signed with the private key found in the working directory.
        /// The test result is written to the Colsole.
        /// </summary>
        static void TestSigning()
        {
            String dirOfExecutable = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            String tmpDocumentPath = Path.Combine(dirOfExecutable, "test.xml");

            // Sign a test document with the private key and serialize the document to the disk
            {
                RSA priv = GetPrivateKey();
                string xmlmessage = @"
                    <root>  
                        <creditcard>
                            <number>19834209</number>
                            <expiry>02/02/2002</expiry>
                        </creditcard>
                    </root> ";
                XmlDocument xmlDoc = new XmlDocument
                {
                    PreserveWhitespace = true
                };
                xmlDoc.LoadXml(xmlmessage);
                Utils.SignXml(xmlDoc, priv);
                // xmlDoc.GetElementsByTagName("number")[0].InnerText = "123123";
                // xmlDoc.GetElementsByTagName("root")[0].AppendChild(xmlDoc.CreateNode("element", "test", ""));
                xmlDoc.Save(tmpDocumentPath);
            }

            // Load the test document from the disk and verify the signature with the embedded public key
            {
                XmlDocument xmlDoc = new XmlDocument
                {
                    PreserveWhitespace = true
                };
                xmlDoc.Load(tmpDocumentPath);
                try
                {
                    Utils.VerifyXml(xmlDoc, Utils.GetEmbeddedPubKey());
                    Console.WriteLine("The XML signature is valid.");
                    Console.WriteLine(xmlDoc.OuterXml);
                }
                catch (Exception e)
                {
                    Console.WriteLine("The XML signature is not valid.");
                    Console.WriteLine(e.Message);
                    Console.WriteLine(xmlDoc.OuterXml);
                }
            }
        }

        /// <summary>
        /// Creates a manifest that can be used to update to the current executed assembly.
        /// The manifest is saved to the current working directory.
        /// The manifest is not signed so the user can set the info etc.
        /// </summary>
        static void CreateUpdateManifest()
        {
            string executable = System.Reflection.Assembly.GetEntryAssembly().Location;
            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
            string hash = Utils.SHA256OfFile(executable);

            string manifest = $@"<manifest>
    <update>
        <version>{version}</version>
        <date>{DateTime.UtcNow:s}</date>
        <info>New release with bug fixes and new features</info>
        <download>https://github.com/Phaiax/PxKeystrokesForScreencasts/raw/master/Releases/{version}/{Path.GetFileName(executable)}</download>
        <download2>https://invisibletower.de/pxkeystrokesforscreencasts/{Path.GetFileName(executable)}</download2>
        <sha256>{hash}</sha256>
    </update>
</manifest>";

            XmlDocument xmlDoc = new XmlDocument
            {
                PreserveWhitespace = true
            };
            xmlDoc.LoadXml(manifest);
            xmlDoc.Save(PATH_UPDATE_MANIFEST);
        }

        /// <summary>
        /// Signs the manifest in the current working directory.
        /// </summary>
        static void SignUpdateManifest()
        {
            XmlDocument xmlDoc = new XmlDocument
            {
                PreserveWhitespace = true
            };
            xmlDoc.Load(PATH_UPDATE_MANIFEST);
            Utils.SignXml(xmlDoc, GetPrivateKey());
            xmlDoc.Save(PATH_UPDATE_MANIFEST);
            Console.WriteLine($"scp {PATH_UPDATE_MANIFEST} $SSHSERVER:html/pxkeystrokesforscreencasts/");

            string executable = System.Reflection.Assembly.GetEntryAssembly().Location;
            Console.WriteLine($"scp /{executable.Replace("\\", "/").Replace(":", "")} $SSHSERVER:html/pxkeystrokesforscreencasts/");
        }

        /// <summary>
        /// Download and verify the manifest and the executable.
        /// </summary>
        static void DownloadAndVerifyManifestAndUpdate()
        {
            try
            {
                Console.WriteLine("Downloading manifest ...");

                XmlDocument manifest = Utils.DownloadAndVerifyManifest();
                Console.WriteLine("Downloading update ...");
                byte[] data = Utils.DownloadExecutableAndVerifyHash(manifest, (e) =>
                {
                    Console.Write($"\r {e}%");
                }, new TimeSpan(0, 0, 10));
                Console.WriteLine();
                Console.WriteLine($"Manifest and update ({data.Length} bytes) are ok.");
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR:");
                Console.WriteLine(e.ToString());
            }
        }
    }
}
