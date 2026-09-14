using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace WeatherTaskbarApp.UI;

public class WeatherIconGenerator
{
    public static Icon GenerateWeatherIcon(string condition, int temperature, string unit)
    {
        const int size = 32;
        using var bitmap = new Bitmap(size, size);
        using var graphics = Graphics.FromImage(bitmap);

        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

        // Transparent background for cleaner look
        graphics.Clear(Color.Transparent);

        // Draw weather symbol based on condition with solid colors
        DrawWeatherSymbolClear(graphics, condition, size);

        // Convert bitmap to icon
        IntPtr hIcon = bitmap.GetHicon();
        Icon icon = Icon.FromHandle(hIcon);

        return icon;
    }

    public static Bitmap GenerateSmallWeatherIcon(string condition, int size = 20)
    {
        var bitmap = new Bitmap(size, size);
        using var graphics = Graphics.FromImage(bitmap);

        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
        graphics.Clear(Color.Transparent);

        // Draw weather symbol
        DrawWeatherSymbolClear(graphics, condition, size);

        return bitmap;
    }

    public static Icon GenerateTrayIcon(int temperature, string unit = "fahrenheit")
    {
        const int size = 32;
        using var bitmap = new Bitmap(size, size);
        using var graphics = Graphics.FromImage(bitmap);

        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        graphics.CompositingQuality = CompositingQuality.HighQuality;

        // Rounded rectangle background with gradient
        using (var path = new GraphicsPath())
        {
            int radius = 6;
            path.AddArc(0, 0, radius * 2, radius * 2, 180, 90);
            path.AddArc(size - radius * 2, 0, radius * 2, radius * 2, 270, 90);
            path.AddArc(size - radius * 2, size - radius * 2, radius * 2, radius * 2, 0, 90);
            path.AddArc(0, size - radius * 2, radius * 2, radius * 2, 90, 90);
            path.CloseFigure();

            // Gradient from dark blue to lighter blue
            using var gradientBrush = new LinearGradientBrush(
                new Point(0, 0),
                new Point(0, size),
                Color.FromArgb(45, 85, 145),   // Darker navy blue top
                Color.FromArgb(70, 130, 200)); // Lighter blue bottom
            graphics.FillPath(gradientBrush, path);

            // Subtle border
            using var borderPen = new Pen(Color.FromArgb(100, 255, 255, 255), 1f);
            graphics.DrawPath(borderPen, path);
        }

        // Temperature with degree symbol (°F or °C)
        string degreeSymbol = unit.ToLower() == "celsius" ? "°" : "°";
        string tempText = temperature.ToString() + degreeSymbol;

        // Use a clean, readable font
        using var font = new Font("Segoe UI", 14, FontStyle.Bold, GraphicsUnit.Pixel);
        var textSize = graphics.MeasureString(tempText, font);

        // Center the text
        float x = (size - textSize.Width) / 2;
        float y = (size - textSize.Height) / 2;

        // Draw text with subtle shadow for depth
        using (var shadowBrush = new SolidBrush(Color.FromArgb(80, 0, 0, 0)))
        {
            graphics.DrawString(tempText, font, shadowBrush, x + 1, y + 1);
        }

        // White text for contrast
        using (var textBrush = new SolidBrush(Color.White))
        {
            graphics.DrawString(tempText, font, textBrush, x, y);
        }

        IntPtr hIcon = bitmap.GetHicon();
        return Icon.FromHandle(hIcon);
    }

    private static void DrawWeatherSymbolClear(Graphics g, string condition, int size)
    {
        float centerX = size / 2f;
        float centerY = size / 2f;

        switch (condition.ToLower())
        {
            case "sunny":
            case "clear":
                DrawSunClear(g, centerX, centerY, size / 2.5f);
                break;

            case "cloudy":
                DrawCloudClear(g, centerX, centerY, size / 2f);
                break;

            case "rainy":
                DrawRainClear(g, centerX, centerY, size / 2f);
                break;

            case "snowy":
                DrawCloudClear(g, centerX, centerY - 3, size / 2.5f);
                DrawSnowClear(g, centerX, centerY + 8, size / 4f);
                break;

            case "stormy":
                DrawCloudClear(g, centerX, centerY - 3, size / 2.5f);
                DrawLightningClear(g, centerX, centerY + 6, size / 3f);
                break;

            case "foggy":
                DrawFogClear(g, centerX, centerY, size / 1.5f);
                break;

            default:
                DrawSunClear(g, centerX, centerY, size / 2.5f);
                break;
        }
    }

    private static void DrawSunClear(Graphics g, float x, float y, float radius)
    {
        // Draw bright yellow sun
        using var brush = new SolidBrush(Color.FromArgb(255, 204, 0));
        g.FillEllipse(brush, x - radius, y - radius, radius * 2, radius * 2);

        // Draw sun rays
        using var pen = new Pen(Color.FromArgb(255, 204, 0), 2.5f);
        for (int i = 0; i < 8; i++)
        {
            double angle = i * Math.PI / 4;
            float x1 = x + (float)(Math.Cos(angle) * radius * 1.2);
            float y1 = y + (float)(Math.Sin(angle) * radius * 1.2);
            float x2 = x + (float)(Math.Cos(angle) * radius * 1.7);
            float y2 = y + (float)(Math.Sin(angle) * radius * 1.7);
            g.DrawLine(pen, x1, y1, x2, y2);
        }
    }

    private static void DrawCloudClear(Graphics g, float x, float y, float size)
    {
        // Use overlapping ellipses for a clean, prominent cloud shape
        float s = size * 1.3f; // Scale up for prominence

        // Gradient brush for depth
        using var gradientBrush = new System.Drawing.Drawing2D.LinearGradientBrush(
            new PointF(x, y - s * 0.5f),
            new PointF(x, y + s * 0.4f),
            Color.FromArgb(240, 240, 240),  // lighter top
            Color.FromArgb(190, 190, 190)); // darker bottom

        // Draw overlapping ellipses to form cloud (no outlines for clean look)
        // Base/bottom ellipse - wide and flat
        g.FillEllipse(gradientBrush, x - s * 0.7f, y - s * 0.05f, s * 1.4f, s * 0.5f);

        // Left puff
        g.FillEllipse(gradientBrush, x - s * 0.65f, y - s * 0.35f, s * 0.55f, s * 0.5f);

        // Center-left puff
        g.FillEllipse(gradientBrush, x - s * 0.3f, y - s * 0.5f, s * 0.55f, s * 0.55f);

        // Center puff (tallest)
        g.FillEllipse(gradientBrush, x - s * 0.05f, y - s * 0.55f, s * 0.6f, s * 0.6f);

        // Center-right puff
        g.FillEllipse(gradientBrush, x + s * 0.25f, y - s * 0.45f, s * 0.5f, s * 0.5f);

        // Right puff (completes the right edge)
        g.FillEllipse(gradientBrush, x + s * 0.45f, y - s * 0.25f, s * 0.45f, s * 0.45f);
    }

    private static void DrawRainClear(Graphics g, float x, float y, float size)
    {
        using var brush = new SolidBrush(Color.FromArgb(70, 130, 180));
        using var outlinePen = new Pen(Color.FromArgb(50, 100, 150), 1f);

        // Large, prominent raindrops arranged in a pattern
        float[] dropOffsetsX = { -0.6f, 0.5f, -0.2f, 0.7f, -0.7f }; // x offsets
        float[] dropOffsetsY = { -0.5f, -0.3f, 0.2f, 0.3f, 0.4f }; // y offsets - staggered
        float[] dropSizes = { 1.0f, 0.9f, 1.1f, 0.85f, 0.75f }; // size multipliers

        for (int i = 0; i < 5; i++)
        {
            float dropX = x + dropOffsetsX[i] * size;
            float dropY = y + dropOffsetsY[i] * size;
            float dropHeight = size * 0.7f * dropSizes[i];
            float dropWidth = dropHeight * 0.5f;
            float angle = 15f; // tilt angle

            // Draw teardrop shape using GraphicsPath
            using var dropPath = new System.Drawing.Drawing2D.GraphicsPath();

            // Teardrop: pointed top, rounded bottom
            float tipY = dropY - dropHeight / 2;
            float bottomY = dropY + dropHeight / 2;
            float bulbRadius = dropWidth / 2;

            // Create smooth teardrop curve
            dropPath.AddLine(dropX, tipY, dropX - bulbRadius, bottomY - bulbRadius);
            dropPath.AddArc(dropX - bulbRadius, bottomY - bulbRadius * 2, bulbRadius * 2, bulbRadius * 2, 180, -180);
            dropPath.AddLine(dropX + bulbRadius, bottomY - bulbRadius, dropX, tipY);
            dropPath.CloseFigure();

            // Apply rotation for falling rain effect
            using var matrix = new System.Drawing.Drawing2D.Matrix();
            matrix.RotateAt(angle, new PointF(dropX, dropY));
            dropPath.Transform(matrix);

            g.FillPath(brush, dropPath);
            g.DrawPath(outlinePen, dropPath);
        }
    }

    private static void DrawSnowClear(Graphics g, float x, float y, float size)
    {
        using var pen = new Pen(Color.FromArgb(100, 149, 237), 1.5f); // Cornflower blue
        using var brush = new SolidBrush(Color.FromArgb(100, 149, 237));

        // Draw multiple snowflakes at different positions and sizes
        float[] flakeX = { -0.5f, 0.5f, 0f, -0.6f, 0.6f };
        float[] flakeY = { -0.4f, -0.2f, 0.3f, 0.4f, 0.5f };
        float[] flakeSizes = { 0.9f, 0.8f, 1.0f, 0.7f, 0.75f };

        for (int i = 0; i < 5; i++)
        {
            float fx = x + flakeX[i] * size;
            float fy = y + flakeY[i] * size;
            float fs = size * 0.35f * flakeSizes[i];
            DrawSnowflake(g, pen, brush, fx, fy, fs);
        }
    }

    private static void DrawSnowflake(Graphics g, Pen pen, Brush brush, float x, float y, float size)
    {
        // Draw 6-pointed snowflake with arms and small branches
        float armLength = size;
        float branchLength = size * 0.35f;
        float branchOffset = size * 0.5f;

        for (int i = 0; i < 6; i++)
        {
            float angle = i * 60f * (float)Math.PI / 180f;
            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);

            // Main arm
            float endX = x + cos * armLength;
            float endY = y + sin * armLength;
            g.DrawLine(pen, x, y, endX, endY);

            // Small branches on each arm
            float branchX = x + cos * branchOffset;
            float branchY = y + sin * branchOffset;

            // Branch at +45 degrees from arm
            float branchAngle1 = angle + 45f * (float)Math.PI / 180f;
            g.DrawLine(pen, branchX, branchY,
                branchX + (float)Math.Cos(branchAngle1) * branchLength,
                branchY + (float)Math.Sin(branchAngle1) * branchLength);

            // Branch at -45 degrees from arm
            float branchAngle2 = angle - 45f * (float)Math.PI / 180f;
            g.DrawLine(pen, branchX, branchY,
                branchX + (float)Math.Cos(branchAngle2) * branchLength,
                branchY + (float)Math.Sin(branchAngle2) * branchLength);
        }

        // Center dot
        g.FillEllipse(brush, x - 2, y - 2, 4, 4);
    }

    private static void DrawLightningClear(Graphics g, float x, float y, float size)
    {
        using var brush = new SolidBrush(Color.FromArgb(255, 215, 0));
        using var pen = new Pen(Color.FromArgb(255, 165, 0), 2f);

        PointF[] points = new PointF[]
        {
            new PointF(x - size / 3, y),
            new PointF(x, y + size / 2),
            new PointF(x - size / 6, y + size / 2),
            new PointF(x + size / 3, y + size)
        };
        g.DrawLines(pen, points);
    }

    private static void DrawFogClear(Graphics g, float x, float y, float size)
    {
        using var pen = new Pen(Color.FromArgb(180, 180, 180), 3f);
        for (int i = -1; i <= 1; i++)
        {
            float fogY = y + i * size / 4;
            g.DrawLine(pen, x - size / 2, fogY, x + size / 2, fogY);
        }
    }

    private static void DrawSun(Graphics g, float x, float y, float radius)
    {
        using var brush = new SolidBrush(Color.FromArgb(255, 255, 200));
        g.FillEllipse(brush, x - radius, y - radius, radius * 2, radius * 2);

        using var pen = new Pen(Color.FromArgb(255, 255, 150), 1.5f);
        for (int i = 0; i < 8; i++)
        {
            double angle = i * Math.PI / 4;
            float x1 = x + (float)(Math.Cos(angle) * radius * 1.3);
            float y1 = y + (float)(Math.Sin(angle) * radius * 1.3);
            float x2 = x + (float)(Math.Cos(angle) * radius * 1.7);
            float y2 = y + (float)(Math.Sin(angle) * radius * 1.7);
            g.DrawLine(pen, x1, y1, x2, y2);
        }
    }

    private static void DrawCloud(Graphics g, float x, float y, float size)
    {
        using var brush = new SolidBrush(Color.FromArgb(240, 240, 255));
        float r1 = size * 0.5f;
        float r2 = size * 0.6f;
        float r3 = size * 0.4f;

        g.FillEllipse(brush, x - size / 2, y, r1 * 2, r1 * 2);
        g.FillEllipse(brush, x - r2 / 2, y - r2 / 2, r2 * 2, r2 * 2);
        g.FillEllipse(brush, x + size / 4, y, r3 * 2, r3 * 2);
    }

    private static void DrawRain(Graphics g, float x, float y, float size)
    {
        using var pen = new Pen(Color.LightBlue, 2);
        for (int i = -1; i <= 1; i++)
        {
            float dropX = x + i * size;
            g.DrawLine(pen, dropX, y, dropX, y + size);
        }
    }

    private static void DrawSnow(Graphics g, float x, float y, float size)
    {
        using var brush = new SolidBrush(Color.White);
        for (int i = -1; i <= 1; i++)
        {
            float snowX = x + i * size;
            g.FillEllipse(brush, snowX - 2, y - 2, 4, 4);
            g.FillEllipse(brush, snowX - 2, y + size - 2, 4, 4);
        }
    }

    private static void DrawLightning(Graphics g, float x, float y, float size)
    {
        using var pen = new Pen(Color.Yellow, 2);
        PointF[] points = new PointF[]
        {
            new PointF(x - size / 4, y),
            new PointF(x, y + size / 2),
            new PointF(x - size / 6, y + size / 2),
            new PointF(x + size / 4, y + size)
        };
        g.DrawLines(pen, points);
    }

    private static void DrawFog(Graphics g, float x, float y, float size)
    {
        using var pen = new Pen(Color.FromArgb(200, 220, 220, 220), 2);
        for (int i = -1; i <= 1; i++)
        {
            float fogY = y + i * size / 3;
            g.DrawLine(pen, x - size, fogY, x + size, fogY);
        }
    }

    private static Color GetBackgroundColor(int temperature, string unit)
    {
        int tempF = unit.ToLower() == "celsius" ? (int)(temperature * 9.0 / 5.0 + 32) : temperature;

        return tempF switch
        {
            < 32 => Color.FromArgb(100, 135, 206),
            < 50 => Color.FromArgb(70, 130, 180),
            < 70 => Color.FromArgb(60, 179, 113),
            < 85 => Color.FromArgb(255, 165, 0),
            _ => Color.FromArgb(220, 20, 60)
        };
    }
}
