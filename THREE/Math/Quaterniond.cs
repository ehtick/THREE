﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace THREE
{
    [Serializable]
    public class Quaterniond : IEquatable<Quaterniond>, ICloneable, INotifyPropertyChanged
    {
        private double _x;
        private double _y;
        private double _z;
        private double _w;

        public double X
        {
            get
            {
                return _x;
            }
            set
            {
                _x = value;
                OnPropertyChanged();
            }
        }

        public double Y
        {
            get
            {
                return _y;
            }
            set
            {
                _y = value;
                OnPropertyChanged();
            }
        }

        public double Z
        {
            get
            {
                return _z;
            }
            set
            {
                _z = value;
                OnPropertyChanged();
            }
        }

        public double W
        {
            get
            {
                return _w;
            }
            set
            {
                _w = value;
                OnPropertyChanged();
            }
        }


        public Quaterniond()
        {
            this.X = this.Y = this.Z = 0;
            this.W = 1;
        }
        public Quaterniond(double x, double y, double z, double w)
        {
            this._x = x;
            this._y = y;
            this._z = z;
            this._w = w;
        }
        public static Quaterniond Identity()
        {
            return new Quaterniond();
        }
        public void Set(double x, double y, double z, double w)
        {
            this._x = x;
            this._y = y;
            this._z = z;
            this._w = w;

            OnPropertyChanged();
        }

        public object Clone()
        {
            return new Quaterniond(this.X, this.Y, this.Z, this.W);
        }

        public Quaterniond Copy(Quaterniond Quaterniond)
        {
            this._x = Quaterniond.X;
            this._y = Quaterniond.Y;
            this._z = Quaterniond.Z;
            this._w = Quaterniond.W;

            this.OnPropertyChanged();

            return this;
        }

        public Quaterniond SetFromEuler(Eulerd euler, bool update = true)
        {
            double x = euler.X, y = euler.Y, z = euler.Z;
            RotationOrder order = euler.Order;

            // http://www.mathworks.com/matlabcentral/fileexchange/
            // 	20696-function-to-convert-between-dcm-euler-angles-Quaternionds-and-euler-vectors/
            //	content/SpinCalc.m



            var c1 = System.Math.Cos(x / 2);
            var c2 = System.Math.Cos(y / 2);
            var c3 = System.Math.Cos(z / 2);

            var s1 = System.Math.Sin(x / 2);
            var s2 = System.Math.Sin(y / 2);
            var s3 = System.Math.Sin(z / 2);

            if (order == RotationOrder.XYZ)
            {

                this._x = s1 * c2 * c3 + c1 * s2 * s3;
                this._y = c1 * s2 * c3 - s1 * c2 * s3;
                this._z = c1 * c2 * s3 + s1 * s2 * c3;
                this._w = c1 * c2 * c3 - s1 * s2 * s3;

            }
            else if (order == RotationOrder.YXZ)
            {

                this._x = s1 * c2 * c3 + c1 * s2 * s3;
                this._y = c1 * s2 * c3 - s1 * c2 * s3;
                this._z = c1 * c2 * s3 - s1 * s2 * c3;
                this._w = c1 * c2 * c3 + s1 * s2 * s3;

            }
            else if (order == RotationOrder.ZXY)
            {

                this._x = s1 * c2 * c3 - c1 * s2 * s3;
                this._y = c1 * s2 * c3 + s1 * c2 * s3;
                this._z = c1 * c2 * s3 + s1 * s2 * c3;
                this._w = c1 * c2 * c3 - s1 * s2 * s3;

            }
            else if (order == RotationOrder.ZYX)
            {

                this._x = s1 * c2 * c3 - c1 * s2 * s3;
                this._y = c1 * s2 * c3 + s1 * c2 * s3;
                this._z = c1 * c2 * s3 - s1 * s2 * c3;
                this._w = c1 * c2 * c3 + s1 * s2 * s3;

            }
            else if (order == RotationOrder.YZX)
            {

                this._x = s1 * c2 * c3 + c1 * s2 * s3;
                this._y = c1 * s2 * c3 + s1 * c2 * s3;
                this._z = c1 * c2 * s3 - s1 * s2 * c3;
                this._w = c1 * c2 * c3 - s1 * s2 * s3;

            }
            else if (order == RotationOrder.XZY)
            {

                this._x = s1 * c2 * c3 - c1 * s2 * s3;
                this._y = c1 * s2 * c3 - s1 * c2 * s3;
                this._z = c1 * c2 * s3 + s1 * s2 * c3;
                this._w = c1 * c2 * c3 + s1 * s2 * s3;

            }

            if (update != false) this.OnPropertyChanged();

            return this;
        }

        public Quaterniond SetFromAxisAngle(Vector3d axis, double angle)
        {
            // http://www.euclideanspace.com/maths/geometry/rotations/conversions/angleToQuaterniond/index.htm

            // assumes axis is normalized

            var halfAngle = angle / 2;
            double s = System.Math.Sin(halfAngle);

            this._x = axis.X * s;
            this._y = axis.Y * s;
            this._z = axis.Z * s;
            this._w = System.Math.Cos(halfAngle);

            OnPropertyChanged();

            return this;
        }

        public Quaterniond SetFromRotationMatrix(Matrix4d m)
        {
            // http://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToQuaterniond/index.htm

            // assumes the upper 3x3 of m is a pure rotation matrix (i.e, unscaled)

            var te = m.Elements;

            double m11 = te[0], m12 = te[4], m13 = te[8],
                  m21 = te[1], m22 = te[5], m23 = te[9],
                  m31 = te[2], m32 = te[6], m33 = te[10],
                  trace = m11 + m22 + m33,
                  s;

            if (trace > 0)
            {

                s = 0.5f / System.Math.Sqrt(trace + 1.0);

                this._w = 0.25f / s;
                this._x = (m32 - m23) * s;
                this._y = (m13 - m31) * s;
                this._z = (m21 - m12) * s;

            }
            else if (m11 > m22 && m11 > m33)
            {

                s = 2.0f * System.Math.Sqrt(1.0 + m11 - m22 - m33);

                this._w = (m32 - m23) / s;
                this._x = 0.25f * s;
                this._y = (m12 + m21) / s;
                this._z = (m13 + m31) / s;

            }
            else if (m22 > m33)
            {

                s = 2.0f * System.Math.Sqrt(1.0 + m22 - m11 - m33);

                this._w = (m13 - m31) / s;
                this._x = (m12 + m21) / s;
                this._y = 0.25f * s;
                this._z = (m23 + m32) / s;

            }
            else
            {

                s = 2.0f * System.Math.Sqrt(1.0 + m33 - m11 - m22);

                this._w = (m21 - m12) / s;
                this._x = (m13 + m31) / s;
                this._y = (m23 + m32) / s;
                this._z = 0.25f * s;

            }

            this.OnPropertyChanged();

            return this;
        }

        public Quaterniond SetFromUnitVectors(Vector3d vFrom, Vector3d vTo)
        {
            // assumes direction vectors vFrom and vTo are normalized

            var EPS = 0.000001;

            var r = vFrom.Dot(vTo) + 1;

            if (r < EPS)
            {

                r = 0;

                if (System.Math.Abs(vFrom.X) > System.Math.Abs(vFrom.Z))
                {

                    this._x = -vFrom.Y;
                    this._y = vFrom.X;
                    this._z = 0;
                    this._w = r;

                }
                else
                {

                    this._x = 0;
                    this._y = -vFrom.Z;
                    this._z = vFrom.Y;
                    this._w = r;

                }

            }
            else
            {

                // crossVectors( vFrom, vTo ); // inlined to avoid cyclic dependency on Vector3d

                this._x = vFrom.Y * vTo.Z - vFrom.Z * vTo.Y;
                this._y = vFrom.Z * vTo.X - vFrom.X * vTo.Z;
                this._z = vFrom.X * vTo.Y - vFrom.Y * vTo.X;
                this._w = r;

            }

            return this.Normalize();
        }

        public double AngleTo(Quaterniond q)
        {
            return 2 * System.Math.Acos(System.Math.Abs(this.Dot(q).Clamp(-1, 1)));
        }

        public Quaterniond RotateTowards(Quaterniond q, double step)
        {
            var angle = this.AngleTo(q);

            if (angle == 0) return this;

            var t = System.Math.Min(1, step / angle);

            this.Slerp(q, t);

            return this;
        }

        public Quaterniond Inverse()
        {
            return this.Conjugate();
        }

        public Quaterniond Conjugate()
        {
            this._x *= -1;
            this._y *= -1;
            this._z *= -1;

            this.OnPropertyChanged();

            return this;
        }
        public double Dot(Quaterniond v)
        {
            return this._x * v.X + this._y * v.Y + this._z * v.Z + this._w * v.W;
        }

        public double LengthSq()
        {
            return this._x * this._x + this._y * this._y + this._z * this._z + this._w * this._w;
        }

        public double Length()
        {
            return System.Math.Sqrt(this._x * this._x + this._y * this._y + this._z * this._z + this._w * this._w);
        }

        public Quaterniond Normalize()
        {
            var l = this.Length();

            if (l == 0)
            {
                this._x = 0;
                this._y = 0;
                this._z = 0;
                this._w = 1;
            }
            else
            {
                l = 1 / l;

                this._x = this._x * l;
                this._y = this._y * l;
                this._z = this._z * l;
                this._w = this._w * l;
            }

            this.OnPropertyChanged();

            return this;
        }

        public Quaterniond Multiply(Quaterniond q)
        {
            return this.MultiplyQuaternionds(this, q);
        }

        public Quaterniond PreMultiply(Quaterniond q)
        {
            return this.MultiplyQuaternionds(q, this);
        }

        public static Quaterniond operator *(Quaterniond a, Quaterniond b)
        {
            Quaterniond r = new Quaterniond();

            double qax = a.X, qay = a.Y, qaz = a.Z, qaw = a.W;
            double qbx = b.X, qby = b.Y, qbz = b.Z, qbw = b.W;

            r._x = qax * qbw + qaw * qbx + qay * qbz - qaz * qby;
            r._y = qay * qbw + qaw * qby + qaz * qbx - qax * qbz;
            r._z = qaz * qbw + qaw * qbz + qax * qby - qay * qbx;
            r._w = qaw * qbw - qax * qbx - qay * qby - qaz * qbz;

            return r;
        }

        public Quaterniond MultiplyQuaternionds(Quaterniond a, Quaterniond b)
        {

            // from http://www.euclideanspace.com/maths/algebra/realNormedAlgebra/Quaternionds/code/index.htm

            double qax = a.X, qay = a.Y, qaz = a.Z, qaw = a.W;
            double qbx = b.X, qby = b.Y, qbz = b.Z, qbw = b.W;

            this._x = qax * qbw + qaw * qbx + qay * qbz - qaz * qby;
            this._y = qay * qbw + qaw * qby + qaz * qbx - qax * qbz;
            this._z = qaz * qbw + qaw * qbz + qax * qby - qay * qbx;
            this._w = qaw * qbw - qax * qbx - qay * qby - qaz * qbz;

            this.OnPropertyChanged();

            return this;

        }
        public Quaterniond Slerp(Quaterniond qa, Quaterniond qb, Quaterniond qm, double t)
        {
            return qm.Copy(qa).Slerp(qb, t);
        }
        public Quaterniond Slerp(Quaterniond qb, double t)
        {

            if (t == 0) return this;
            if (t == 1) return this.Copy(qb);

            double x = this._x, y = this._y, z = this._z, w = this._w;

            // http://www.euclideanspace.com/maths/algebra/realNormedAlgebra/Quaternionds/slerp/

            var cosHalfTheta = w * qb.W + x * qb.X + y * qb.Y + z * qb.Z;

            if (cosHalfTheta < 0)
            {

                this._w = -qb.W;
                this._x = -qb.X;
                this._y = -qb.Y;
                this._z = -qb.Z;

                cosHalfTheta = -cosHalfTheta;

            }
            else
            {

                this.Copy(qb);

            }

            if (cosHalfTheta >= 1.0)
            {

                this._w = w;
                this._x = x;
                this._y = y;
                this._z = z;

                return this;

            }

            var sqrSinHalfTheta = 1.0 - cosHalfTheta * cosHalfTheta;

            if (sqrSinHalfTheta <= double.Epsilon)
            {

                var s = 1 - t;
                this._w = s * w + t * this._w;
                this._x = s * x + t * this._x;
                this._y = s * y + t * this._y;
                this._z = s * z + t * this._z;

                this.Normalize();
                this.OnPropertyChanged();

                return this;

            }

            var sinHalfTheta = System.Math.Sqrt(sqrSinHalfTheta);
            var halfTheta = System.Math.Atan2(sinHalfTheta, cosHalfTheta);
            double ratioA = System.Math.Sin((1 - t) * halfTheta) / sinHalfTheta,
                ratioB = System.Math.Sin(t * halfTheta) / sinHalfTheta;

            this._w = (w * ratioA + this._w * ratioB);
            this._x = (x * ratioA + this._x * ratioB);
            this._y = (y * ratioA + this._y * ratioB);
            this._z = (z * ratioA + this._z * ratioB);

            this.OnPropertyChanged();

            return this;

        }

        public void SlerpFlat(double[] dst, int dstOffset, double[] src0, int srcOffset0, double[] src1, int srcOffset1, double t)
        {

            // fuzz-free, array-based Quaterniond SLERP operation

            double x0 = src0[srcOffset0 + 0],
                y0 = src0[srcOffset0 + 1],
                z0 = src0[srcOffset0 + 2],
                w0 = src0[srcOffset0 + 3],

                x1 = src1[srcOffset1 + 0],
                y1 = src1[srcOffset1 + 1],
                z1 = src1[srcOffset1 + 2],
                w1 = src1[srcOffset1 + 3];

            if (w0 != w1 || x0 != x1 || y0 != y1 || z0 != z1)
            {
                double s = 1 - t,
                    cos = x0 * x1 + y0 * y1 + z0 * z1 + w0 * w1,
                    dir = (cos >= 0 ? 1 : -1),
                    sqrSin = 1 - cos * cos;

                // Skip the Slerp for tiny steps to avoid numeric problems:
                if (sqrSin > double.Epsilon)
                {

                    double sin = System.Math.Sqrt(sqrSin),
                        len = System.Math.Atan2(sin, cos * dir);

                    s = System.Math.Sin(s * len) / sin;
                    t = System.Math.Sin(t * len) / sin;

                }

                var tDir = t * dir;

                x0 = x0 * s + x1 * tDir;
                y0 = y0 * s + y1 * tDir;
                z0 = z0 * s + z1 * tDir;
                w0 = w0 * s + w1 * tDir;

                // Normalize in case we just did a lerp:
                if (s == 1 - t)
                {

                    var f = 1 / System.Math.Sqrt(x0 * x0 + y0 * y0 + z0 * z0 + w0 * w0);

                    x0 *= f;
                    y0 *= f;
                    z0 *= f;
                    w0 *= f;

                }

            }

            dst[dstOffset] = x0;
            dst[dstOffset + 1] = y0;
            dst[dstOffset + 2] = z0;
            dst[dstOffset + 3] = w0;
        }

        public bool Equals(Quaterniond Quaterniond)
        {
            return (Quaterniond.X == this._x) && (Quaterniond.Y == this._y) && (Quaterniond.Z == this._z) && (Quaterniond.W == this._w);
        }

        public Quaterniond FromArray(double[] array, int? offset = null)
        {
            int index = 0;
            if (offset != null) index = (int)offset;

            this._x = array[index];
            this._y = array[index + 1];
            this._z = array[index + 2];
            this._w = array[index + 3];

            this.OnPropertyChanged();

            return this;

        }

        public double[] ToArray(double[] array = null, int? offset = null)
        {
            int index = 0;
            if (array == null) array = new double[4];
            if (offset != null) index = (int)offset;

            array[index] = this._x;
            array[index + 1] = this._y;
            array[index + 2] = this._z;
            array[index + 3] = this._w;

            return array;

        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
