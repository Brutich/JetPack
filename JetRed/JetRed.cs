using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JetRed
{
    //// WIP.
    //internal class JetGeometry
    //{
    //    internal static double _point_to_polygon_distance(double x, double y, double[][][] polygon)
    //    {
    //        bool inside = false;
    //        double min_dist_sq = Double.PositiveInfinity;

    //        foreach (var ring in polygon)
    //        {
    //            var b = ring.Last();
    //            foreach (var a in ring)
    //            {
    //                if ((a[1] > y) != (b[1] > y) && (x < (b[0] - a[0]) * (y - a[1]) / (b[1] - a[1]) + a[0]))
    //                {
    //                    inside = !inside;
    //                }

    //                min_dist_sq = Math.Min(min_dist_sq, _get_seg_dist_sq(x, y, a, b));
    //                b = a;
    //            }
    //        }

    //        var result = Math.Sqrt(min_dist_sq);
    //        if (!inside)
    //        {
    //            return -result;
    //        }
    //        return result;
    //    }


    //    internal static double _get_seg_dist_sq(double px, double py, double[] a, double[] b)
    //    {
    //        var x = a[0];
    //        var y = a[1];
    //        var dx = b[0] - x;
    //        var dy = b[1] - y;

    //        if (dx != 0 || dy != 0)
    //        {
    //            var t = ((px - x) * dx + (py - y) * dy) / (dx * dx + dy * dy);
    //            if (t > 1)
    //            {
    //                x = b[0];
    //                y = b[1];
    //            }
    //            else if (t > 0)
    //            {
    //                x += dx * t;
    //                y += dy * t;
    //            }
    //        }

    //        dx = px - x;
    //        dy = py - y;

    //        return dx * dx + dy * dy;
    //    }


    //    internal Cell _get_centroid_cell(double[][][] polygon)
    //    {
    //        double area = 0.00;
    //        double x = 0.00;
    //        double y = 0.00;
    //        double[][] points = polygon[0];
    //        double[] b = points.Last();  // prev

    //        foreach (double[] a in points)
    //        {
    //            double f = a[0] * b[1] - b[0] * a[1];
    //            x += (a[0] + b[0]) * f;
    //            y += (a[1] + b[1]) * f;
    //            area += f * 3;
    //            b = a;
    //        }

    //        if (area == 0.00)
    //        {
    //            return new Cell(points[0][0], points[0][1], 0.00, polygon);
    //        }

    //        return new Cell(x / area, y / area, 0.00, polygon);
    //    }


    //    /// <summary>
    //    /// ## Usage
    //    /// from polylabel import polylabel
    //    /// polylabel([[[0, 0], [1, 0], [2, 0], [0, 0]]])  # [0, 0]
    //    /// polylabel([[[0, 0], [1, 0], [1, 1], [0, 1], [0, 0]]], with_distance=True)  # ([0.5, 0.5], 0.5)
    //    /// </summary>
    //    /// <param name=""></param>
    //    /// <param name="precision"></param>
    //    /// <param name="debug"></param>
    //    /// <param name="with_distance"></param>
    //    /// <returns></returns>
    //    internal double[] polylabel(double[][][] polygon, double precision = 1.0, bool debug = false, bool with_distance = false)
    //    {
    //        // Sample of polygon
    //        double[][][] poly = new double[][][]
    //        {
    //           new double[][]
    //           {
    //               new double[]{ 0, 0 },
    //               new double[]{ 1, 0 },
    //               new double[]{ 1, 1 },
    //               new double[]{ 0, 1 }
    //           },
    //        };


    //        // Find bounding box.
    //        var first_item = polygon[0][0];
    //        var min_x = first_item[0];
    //        var min_y = first_item[1];
    //        var max_x = first_item[0];
    //        var max_y = first_item[1];

    //        foreach (var p in polygon[0])
    //        {
    //            if (p[0] < min_x)
    //            {
    //                min_x = p[0];
    //            }
    //            if (p[1] < min_y)
    //            {
    //                min_y = p[1];
    //            }
    //            if (p[0] > max_x)
    //            {
    //                max_x = p[0];
    //            }
    //            if (p[1] > max_y)
    //            {
    //                max_y = p[1];
    //            }
    //        }

    //        var width = max_x - min_x;
    //        var height = max_y - min_y;
    //        var cell_size = Math.Min(width, height);
    //        var h = cell_size / 2.0;

    //        var cell_queue = new PriorityQueue<double>();

    //        if (cell_size == 0)
    //        {
    //            //if (with_distance)
    //            //{
    //            //    return [min_x, min_y], None;
    //            //}
    //            //else
    //            //{
    //                return new double[] { min_x, min_y};
    //            //}
    //        }

    //        // Cover polygon with initial cells.
    //        var x = min_x;
    //        while (x < max_x)
    //        {
    //            var y = min_y;
    //            while (y < max_y)
    //            {
    //                var c = new Cell(x + h, y + h, h, polygon);
    //                y += cell_size;
    //                cell_queue.put((-c.max, time.time(), c));
    //            }

    //            x += cell_size;
    //        }

    //        var best_cell = _get_centroid_cell(polygon);
    //        var bbox_cell = new Cell(min_x + width / 2, min_y + height / 2, 0, polygon);
    //        if (bbox_cell.d > best_cell.d)
    //        {
    //            best_cell = bbox_cell;
    //        }

    //        var num_of_probes = cell_queue.qsize();
    //        while (!cell_queue.empty())
    //        {
    //            _, __, cell = cell_queue.get();

    //            if (cell.d > best_cell.d)
    //            {
    //                best_cell = cell;

    //                if (debug)
    //                {
    //                    MessageBox.Show($"found best {Math.Round(1e4 * cell.d) / 1e4} after {num_of_probes} probes");
    //                }

    //            }

    //            if (cell.max - best_cell.d <= precision)
    //            {
    //                continue;
    //            }

    //            h = cell.h / 2;
    //            var c = new Cell(cell.x - h, cell.y - h, h, polygon);
    //            cell_queue.put((-c.max, time.time(), c));
    //            c = new Cell(cell.x + h, cell.y - h, h, polygon);
    //            cell_queue.put((-c.max, time.time(), c));
    //            c = new Cell(cell.x - h, cell.y + h, h, polygon);
    //            cell_queue.put((-c.max, time.time(), c));
    //            c = new Cell(cell.x + h, cell.y + h, h, polygon);
    //            cell_queue.put((-c.max, time.time(), c));
    //            num_of_probes += 4;
    //        }


    //        if (debug)
    //        {
    //            MessageBox.Show($"num probes: {num_of_probes}");
    //            MessageBox.Show($"best distance: {best_cell.d}");
    //        }

    //        //if (with_distance)
    //        //{
    //        //    return [best_cell.x, best_cell.y], best_cell.d;
    //        //}
    //        //else
    //        //{
    //        return new double[] { best_cell.x, best_cell.y };
    //        //}
    //    }
    //}
    //internal class Cell
    //{
    //    internal double h;
    //    internal double y;
    //    internal double x;
    //    internal double d;
    //    internal double max;

    //    internal Cell(double x, double y, double h, double[][][] polygon)
    //    {
    //        this.h = h;
    //        this.y = y;
    //        this.x = x;
    //        this.d = JetGeometry._point_to_polygon_distance(x, y, polygon);
    //        this.max = this.d + this.h * Math.Sqrt(2);
    //    }

    //    public static bool operator <(Cell f1, Cell f2) => f1.max < f2.max;
    //    public static bool operator <=(Cell f1, Cell f2) => f1.max <= f2.max;
    //    public static bool operator >(Cell f1, Cell f2) => f1.max > f2.max;
    //    public static bool operator >=(Cell f1, Cell f2) => f1.max >= f2.max;
    //    public static bool operator ==(Cell f1, Cell f2) => f1.max == f2.max;
    //    public static bool operator !=(Cell f1, Cell f2) => f1.max != f2.max;
    //}
}

