namespace MathNet.Spatial.Euclidean
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Schema;
    using System.Xml.Serialization;
    using MathNet.Numerics.LinearAlgebra;
    using MathNet.Spatial.Units;

    /// <summary>
    /// Represents a point in 2 dimensional cartesian space
    /// </summary>
    [Serializable]
    public struct Point2D : IXmlSerializable, IEquatable<Point2D>, IFormattable
    {

        /// <summary>
        /// The x coordinate
        /// </summary>
        public readonly double X; // Using public fields cos: http://blogs.msdn.com/b/ricom/archive/2006/08/31/performance-quiz-11-ten-questions-on-value-based-programming.aspx

        /// <summary>
        /// The y coordinate
        /// </summary>
        public readonly double Y; // Using public fields cos: http://blogs.msdn.com/b/ricom/archive/2006/08/31/performance-quiz-11-ten-questions-on-value-based-programming.aspx

        /// <summary>
        /// Creates a point for given coordinates (x, y)
        /// </summary>
        /// <param name="x">The x coordinate</param>
        /// <param name="y">The y coordinate</param>
        public Point2D(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Creates a point r from origin rotated a counterclockwise from X-Axis
        /// </summary>
        /// <param name="r">distance from origin</param>
        /// <param name="a">the angle</param>
        public Point2D(double r, Angle a)
            : this(r * Math.Cos(a.Radians), r * Math.Sin(a.Radians))
        {
        }

        /// <summary>
        /// Creates a point from a list of coordinates (x, y)
        /// </summary>
        /// <param name="data">a pair of coordinates in the order x, y</param>
        /// <exception cref="ArgumentException">Exception thrown if more than 2 coordinates are passed</exception>
        public Point2D(IEnumerable<double> data)
            : this(data.ToArray())
        {
        }

        /// <summary>
        /// Creates a point from a list of coordinates (x, y)
        /// </summary>
        /// <param name="data">a pair of coordinates in the order x, y</param>
        /// <exception cref="ArgumentException">Exception thrown if more than 2 coordinates are passed</exception>
        public Point2D(double[] data)
            : this(data[0], data[1])
        {
            if (data.Length != 2)
            {
                throw new ArgumentException("data.Length != 2!");
            }
        }

        /// <summary>
        /// Returns a point at the origin (0,0)
        /// </summary>
        public static Point2D Origin
        {
            get { return new Point2D(0, 0); }
        }

        /// <summary>
        /// Attempts to convert a string of the form x,y into a point
        /// </summary>
        /// <param name="value">The string to be converted</param>
        /// <returns>A point at the coordinates specified</returns>
        public static Point2D Parse(string value)
        {
            var doubles = Parser.ParseItem2D(value);
            return new Point2D(doubles);
        }

        /// <summary>
        /// Creates a point from xml with x and y coordinates either as attributes or elements
        /// </summary>
        /// <param name="reader">an xml reader</param>
        /// <returns>A point</returns>
        public static Point2D ReadFrom(XmlReader reader)
        {
            var v = new Point2D();
            v.ReadXml(reader);
            return v;
        }

        /// <summary>
        /// Returns the centeroid or center of mass of any set of points
        /// </summary>
        /// <param name="points">a list of points</param>
        /// <returns>the centeroid point</returns>
        public static Point2D Centroid(IEnumerable<Point2D> points)
        {
            return Centroid(points.ToArray());
        }

        /// <summary>
        /// Returns the centeroid or center of mass of any set of points
        /// </summary>
        /// <param name="points">a list of points</param>
        /// <returns>the centeroid point</returns>
        public static Point2D Centroid(params Point2D[] points)
        {
            return new Point2D(
                points.Average(point => point.X),
                points.Average(point => point.Y));
        }

        /// <summary>
        /// Returns a point midway between the provided points <paramref name="point1"/> and <paramref name="point2"/>
        /// </summary>
        /// <param name="point1">point A</param>
        /// <param name="point2">point B</param>
        /// <returns>a new point midway between the provided points</returns>
        public static Point2D MidPoint(Point2D point1, Point2D point2)
        {
            return Centroid(point1, point2);
        }

        public static Point2D operator +(Point2D point, Vector2D vector)
        {
            return new Point2D(point.X + vector.X, point.Y + vector.Y);
        }

        public static Point3D operator +(Point2D point, Vector3D vector)
        {
            return new Point3D(point.X + vector.X, point.Y + vector.Y, vector.Z);
        }

        public static Point2D operator -(Point2D point, Vector2D vector)
        {
            return new Point2D(point.X - vector.X, point.Y - vector.Y);
        }

        public static Point3D operator -(Point2D point, Vector3D vector)
        {
            return new Point3D(point.X - vector.X, point.Y - vector.Y, -1 * vector.Z);
        }

        public static Vector2D operator -(Point2D lhs, Point2D rhs)
        {
            return new Vector2D(lhs.X - rhs.X, lhs.Y - rhs.Y);
        }

        public static bool operator ==(Point2D left, Point2D right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Point2D left, Point2D right)
        {
            return !left.Equals(right);
        }

        public Point2D TransformBy(Matrix<double> m)
        {
            var transformed = m.Multiply(this.ToVector());
            return new Point2D(transformed);
        }

        public override string ToString()
        {
            return this.ToString(null, CultureInfo.InvariantCulture);
        }

        public string ToString(IFormatProvider provider)
        {
            return this.ToString(null, provider);
        }

        public string ToString(string format, IFormatProvider provider = null)
        {
            var numberFormatInfo = provider != null ? NumberFormatInfo.GetInstance(provider) : CultureInfo.InvariantCulture.NumberFormat;
            string separator = numberFormatInfo.NumberDecimalSeparator == "," ? ";" : ",";
            return string.Format("({0}{1} {2})", this.X.ToString(format, numberFormatInfo), separator, this.Y.ToString(format, numberFormatInfo));
        }

        public bool Equals(Point2D other)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return this.X == other.X && this.Y == other.Y;
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        public bool Equals(Point2D other, double tolerance)
        {
            if (tolerance < 0)
            {
                throw new ArgumentException("epsilon < 0");
            }

            return Math.Abs(other.X - this.X) < tolerance &&
                   Math.Abs(other.Y - this.Y) < tolerance;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is Point2D && this.Equals((Point2D)obj);
        }

        /// <summary>
        /// Returns a hashcode for the point
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                return (this.X.GetHashCode() * 397) ^ this.Y.GetHashCode();
            }
        }

        /// <summary>
        /// This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should return null (Nothing in Visual Basic) from this method, and instead, if specifying a custom schema is required, apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute"/> to the class.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Xml.Schema.XmlSchema"/> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)"/> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)"/> method.
        /// </returns>
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// Handles both attribute and element style
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader"/> stream from which the object is deserialized. </param>
        public void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();
            var e = (XElement)XNode.ReadFrom(reader);

            // Hacking set readonly fields here, can't think of a cleaner workaround
            XmlExt.SetReadonlyField(ref this, x => x.X, XmlConvert.ToDouble(e.ReadAttributeOrElementOrDefault("X")));
            XmlExt.SetReadonlyField(ref this, x => x.Y, XmlConvert.ToDouble(e.ReadAttributeOrElementOrDefault("Y")));
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"/> stream to which the object is serialized. </param>
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttribute("X", this.X);
            writer.WriteAttribute("Y", this.Y);
        }

        public Vector2D VectorTo(Point2D otherPoint)
        {
            return otherPoint - this;
        }

        /// <summary>
        /// Finds the straightline distance to another point
        /// </summary>
        /// <param name="otherPoint">The other point</param>
        /// <returns>a distance measure</returns>
        public double DistanceTo(Point2D otherPoint)
        {
            var vector = this.VectorTo(otherPoint);
            return vector.Length;
        }

        /// <summary>
        /// Converts this point into a vector from the origin
        /// </summary>
        /// <returns>A vector equivalent to thsi point</returns>
        public Vector2D ToVector2D()
        {
            return new Vector2D(this.X, this.Y);
        }

        /// <summary>
        /// return new Point3D(X, Y, 0);
        /// </summary>
        /// <returns>return new Point3D(X, Y, 0);</returns>
        public Point3D ToPoint3D()
        {
            return new Point3D(this.X, this.Y, 0);
        }

        public Point3D TransformBy(CoordinateSystem cs)
        {
            return cs.Transform(this.ToPoint3D());
        }

        /// <summary>
        /// Create a new Point2D from a Math.NET Numerics vector of length 2.
        /// </summary>
        public static Point2D OfVector(Vector<double> vector)
        {
            if (vector.Count != 2)
            {
                throw new ArgumentException("The vector length must be 2 in order to convert it to a Point2D");
            }

            return new Point2D(vector.At(0), vector.At(1));
        }

        /// <summary>
        /// Convert to a Math.NET Numerics dense vector of length 2.
        /// </summary>
        public Vector<double> ToVector()
        {
            return Vector<double>.Build.Dense(new[] { X, Y });
        }
    }
}
