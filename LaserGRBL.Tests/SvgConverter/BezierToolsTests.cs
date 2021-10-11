using System;
using System.Linq;
using System.Windows;
using LaserGRBL.SvgConverter;
using Xunit;

namespace LaserGRBL.Tests
{
    public class BezierToolsTests
    {
        [InlineData(new [] { 0.0, 0, 0, 1, 0, 2, 0, 3 } )] /* vertical line, increasing */
        [InlineData(new [] { 0.0, 3, 0, 2, 0, 1, 0, 0 } )] /* vertical line, decreasing */
        [InlineData(new [] { 0.0, 0, 1, 0, 2, 0, 3, 0 } )] /* horizontal line (regression test for stack overflow bug in original BezierTools.cs) */
        [InlineData(new [] { 0.0, 0, 1, 1, 2, 2, 3, 3 } )] /* diagonal */
        [Theory]
        public void FlattenTo_NoSubdivisionWhenLinear_ReturnBaseCaseApproxTriangle(double[] polygon)
        {
            var points = UnflattenArrayToPoints(polygon);
            var result = BezierTools.FlattenTo(points, .001).ToArray();
            
            Assert.Equal(4, result.Length);
            var flattened = FlattenPointsToArray(result);
            Assert.Equal(polygon, flattened);
        }

        [InlineData(new [] { 0.0, 0, 0, 3, 0, 2, 0, 1 })] /* vertical line, point # L-to-R: 1,4,3,2 */
        [InlineData(new[] { 0.0, 0, 2, 0, 3, 0, 1, 0 })] /* horizontal line, point # L-T-R: 1,3,4,2 */
        [Theory]
        public void FlattenTo_NoSubdivisionWhenLinearButOutOfOrder(double[] polygon)
        {
            var points = UnflattenArrayToPoints(polygon);
            var result = BezierTools.FlattenTo(points, .001).ToArray();
            Assert.Equal(4, result.Length);
            var flattened = FlattenPointsToArray(result);
            Assert.Equal(polygon, flattened);
        }

        [InlineData(new double[] { 0, 0, 0, 1, 1, 1, 1, 0 }, 1, (1) * 3)] /* 2^0 * 3 sides */
        [InlineData(new double[] { 0,0,  0,1,  1,1,  1,0 }, 0.5, (2) * 3)] /* 2^1 * 3 sides */
        [InlineData(new double[] { 0, 0, 0, 1, 1, 1, 1, 0 }, 0.25, (2 * 2) * 3)] /* 2^2 * 3 sides */
        [InlineData(new double[] { 0, 0, 0, 1, 1, 1, 1, 0 }, 0.0625, (2 * 2 * 2) * 3)] /* 2^3 * 2 sides */
        [Theory]
        public void FlattenTo_SubdividesToCorrectError(double[] polygon, double acceptedError, int expectedNumSides)
        {
            var points = UnflattenArrayToPoints(polygon);
            var resultingPoints = BezierTools.FlattenTo(points, acceptedError).ToArray();
            Assert.Equal(1 + expectedNumSides, resultingPoints.Length);
        }

        public Point[] UnflattenArrayToPoints(double[] p)
        {
            return new[] {new Point(p[0], p[1]), new Point(p[2], p[3]), new Point(p[4], p[5]), new Point(p[6], p[7])};
        }

        public double[] FlattenPointsToArray(Point[] points)
        {
            return points.SelectMany(p => new [] {p.X, p.Y}).ToArray();
        }
    }
}
