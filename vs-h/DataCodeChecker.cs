using HalconDotNet;
using System;

namespace vs_h
{
    public static class DataCodeReader
    {
        public class Result
        {
            public bool Found { get; set; }
            public string[] DecodedStrings { get; set; } = Array.Empty<string>();
            public HObject SymbolXLDs { get; set; } = null; // nhớ Dispose sau khi dùng
            public double TimeMs { get; set; }
            public string Error { get; set; } = "";
        }

        public static Result ReadInRoi(HImage img, double x, double y, double w, double h, string codeType)
        {
            var res = new Result();

            if (img == null) { res.Error = "Image null"; return res; }
            if (w <= 0 || h <= 0) { res.Error = "ROI invalid"; return res; }

            using (var roi = new HRegion())
            {
                double row1 = y;
                double col1 = x;
                double row2 = y + h;
                double col2 = x + w;
                roi.GenRectangle1(row1, col1, row2, col2);

                using (var imgRoi = img.ReduceDomain(roi))
                {
                    HTuple handle;
                    HOperatorSet.CreateDataCode2dModel(codeType, new HTuple(), new HTuple(), out handle);

                    try
                    {
                        // param tối thiểu để chạy ổn (có thể chỉnh sau)
                        HOperatorSet.SetDataCode2dParam(handle, "polarity", "any");
                        HOperatorSet.SetDataCode2dParam(handle, "mirrored", "no");

                        HTuple t1, t2;
                        HOperatorSet.CountSeconds(out t1);

                        HObject xlds;
                        HTuple resultHandles, decoded;
                        HOperatorSet.FindDataCode2d(imgRoi, out xlds, handle,
                            new HTuple(), new HTuple(), out resultHandles, out decoded);

                        HOperatorSet.CountSeconds(out t2);
                        res.TimeMs = (t2.D - t1.D) * 1000.0;

                        if (decoded == null || decoded.Length == 0)
                        {
                            xlds?.Dispose();
                            res.Found = false;
                            return res;
                        }

                        res.Found = true;
                        res.SymbolXLDs = xlds;

                        string[] arr = new string[decoded.Length];
                        for (int i = 0; i < decoded.Length; i++) arr[i] = decoded[i].S;
                        res.DecodedStrings = arr;

                        return res;
                    }
                    catch (Exception ex)
                    {
                        res.Error = ex.Message;
                        return res;
                    }
                    finally
                    {
                        try { HOperatorSet.ClearDataCode2dModel(handle); } catch { }
                    }
                }
            }
        }
    }
}
