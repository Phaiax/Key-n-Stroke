import numpy as np
import matplotlib.pyplot as plt
import math
from matplotlib.widgets import Slider


#      LabeledSlider.Value                                                  X
#                                                                          XX
#        f(x) ^                                                    +     XX
#             |                                                    |    XX
#             |                                                    |  XX
#   ymax   +--+                                              +------XX------+
#             |                                                    XX
#             |                                                   X|
#             |                                                 XX |
#             |                                                X   +
#             |                                              XX
#             |                                            XXX
#             |                                           XX
#             |                                        XXX
#             |                                      XXX
#             |                                    XX
#             |                         +       XXX
#             |                         |    XXXX
#             |                         | XXXX
#ys(ysper) +--+                   +---XXXX--+
#             |                    XXXX |
#             |                XXXXXX   |
#             |         XXXXXXXX        |
#    ymin  +--XXXXXXXXXXX               |
#             |                         +
#             |
#             |
#          +----------------------------+--------------------------+----------------------->  inner Slider.Value
#             |                         |                          |                       x
#             +                         +                          +
#             x0=0                      xs=x1/2                    x1
#
#
#   y = f(x) = a + b * exp(c * x)
#
#   Calculate a, b, c from the constants ymin, ys, ymax, x1 so that f(x) fulfills (1), (2) and (3).
#
#    (1)       f(x = x0 = 0)      = ymin
#    (2)       f(x = xs = x1 / 2) = ys
#    (3)       f(x = x1)          = ymax
#
#  xs is not free to chose but set to x1/2 so that while solving the equations, we magically get a quadratic equation.
#
#                              (1)                ymin = a + b
#                              (2)                ys   = a + b * exp(c * x1 / 2)
#                              (3)                ymax = a + b * exp(c * x1)
#    
#    (1) for a                 (4)                a    = ymin - b
#    (4) in (2)                (5)                ys   = ymin - b + b * exp(c * x1 / 2)
#    (4) in (3)                (6)                ymax = ymin - b + b * exp(c * x1)
#    -ymin                     (6)         ymax - ymin = b * (exp(c * x1) - 1)
#    solve (5) for b           (7)                b    = (ys - ymin) / (exp(c * x1 / 2) - 1)
#    (7) in (6)                (8)         ymax - ymin = (ys - ymin) / (exp(c * x1 / 2) - 1) * (exp(c * x1) - 1) 
#    *exp(..), -rhs            (8)         (ymax - ymin) * (exp(c * x1 / 2) - 1) - (ys - ymin) * (exp(c * x1) - 1) = 0
#    set                       (9)                   d = (ymin - ymax) / (ys - ymin)
#    substitute (9) in (8)     (10)             exp(c * x1    )      + d * e(c * x1 / 2) + (-1 - d) = 0
#    in first exp: /2*2        (10)             exp(c * x1 / 2) ** 2 + d * e(c * x1 / 2) + (-1 - d) = 0
#    set                       (11)             v = exp(c * x1 / 2)
#    substitute (11) in (10)   (12)				v               ** 2 + d * v             + (-1 - d) == 0
#    solve quadratic eq (12)   (13)             v1,2 = - (d/2) +- sqrt( (d/2)**2 - (-1-d) )
#    solve (11) for c          (14)             c = 2 * ln(g) / x1
#
#    calculate v1 with (13) and (9). Use (14), (7), (4) to calculate a,b,c


fig, ax = plt.subplots()
plt.subplots_adjust(bottom=0.25)




ymax = 120.
ymin = 3.
#ys=
ysper = 0.2 # value from 0 to 1 that gets mapped to the intervall [ymin+1%, ((ymin+ymax/2)-1%] and used as ys

x0=0.
x1=1000.
xs=x1/2


def pq(p, q):
	r1 = (-(p/2))
	r2 = np.sqrt((p/2)**2 - q)
	return (r1 + r2, r1 - r2)

def calc(ysper):
	ysmin = ymin * 1.01
	ysmax = ((ymin+ymax)/2) * 0.99
	ys = ysmin + ysper * (ysmax - ysmin)

	d=(ymin-ymax)/(ys-ymin)
	v1, v2 = pq(d, -1-d)

	c = 2 * np.log(v1) / x1
	b = (ys - ymin) / (np.exp(c*x1/2) - 1)
	a = ymin - b

	t = np.exp(c*x1) + 1 + d * np.exp(c*x1/2) - d
	t1 = (ymax-ymin) * (np.exp(c*x1/2)-1) - (ys-ymin) * (np.exp(c*x1)-1)

	print(f"v1={v1:.3} v2={v2:.3} a={a:.3} b={b:.3} c={c:.3} f(x0)=a+b={a+b:.3}=={ymin:.3}"
		  f" f(xs)=a+bexp(c*x1/2)={a+b*np.exp(c*x1/2):.3}=={ys:.3}"
		  f" f(x1)=a+bexp(c*x1)={a+b*np.exp(c*x1):.3}=={ymax:.3}")

	x = np.arange(x0, x1, (x1-x0)/1000)
	y = a + b * np.exp(c*x)

	return x, y, ys

x, y, ys = calc(ysper)
l1, = plt.plot(x, y)

plt.plot([x0, x1], [ymax, ymax])
plt.plot([x0, x1], [ymin, ymin])
l2, = plt.plot([x0, x1], [ys, ys])

plt.plot([x0, x0], [0, ymax])
plt.plot([x1, x1], [0, ymax])
plt.plot([xs, xs], [0, ymax])


def update(ysper):
	x, y, ys = calc(ysper)
	l1.set_ydata(y)
	l1.set_xdata(x)
	l2.set_ydata([ys, ys])

axys = plt.axes([0.25, 0.1, 0.65, 0.03])
sys = Slider(axys, 'ys %', 0, 1.0, valinit=ysper)

sys.on_changed(update)

plt.show()