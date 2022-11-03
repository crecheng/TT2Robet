﻿using System.Diagnostics.CodeAnalysis;
using System.Drawing;
// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

[SuppressMessage("Interoperability", "CA1416:验证平台兼容性")]
public static class SkillCalTool
{

    public static int ColNum = -1;
    public static int RowNum = -1;
    private static bool _init = false;

    public static void Init()
    {
        if(_init)
            return;
        var data = SkillCalDataManage.Instance;
        var num = data.GetMaxDicColRow();
        ColNum  = num >> 16;
        RowNum = num & 0xffff;
    }
    
    
    public static void DrawSkill(this SkillCalCore core, string file, string imgPath)
    {
        Init();
        var data = SkillCalDataManage.Instance;
        var blocks= data.GetAllBlock();
        Dictionary<string, ItemData[,]> allData = new Dictionary<string, ItemData[,]>();
        foreach (var block in blocks)
        {
            allData.Add(block,data.GetDicMat(block,ColNum,RowNum));
        }

        int blockWidth = ColNum * 128;
        int blockHeight = RowNum * 128;

        var row = (blocks.Count - 1) / 3 + 1;

        Bitmap bitmap = new Bitmap(blockWidth * 3, blockHeight * row);
        Graphics g=Graphics.FromImage(bitmap);
        Font font = new Font(FontFamily.GenericMonospace, 50f,FontStyle.Bold);
        Font fontW = new Font(FontFamily.GenericMonospace, 60f,FontStyle.Bold);
        int b = 0;
        foreach (var (key,mat) in allData)
        {
            int x = b % 3;
            int y = b / 3;
            for (int j = 0; j < RowNum; j++)
            {
                for (int i = 0; i < ColNum; i++)
                {
                    var d = mat[i, j];
                    if (d != null)
                    {
                        if (File.Exists($"{imgPath}{d.Img}.png"))
                        {
                            g.DrawImage(Image.FromFile($"{imgPath}{d.Img}.png"),
                                x * blockWidth + i * 128 + 3, y * blockHeight + j * 128 + 3);
                        }
                        if(core.Point.ContainsKey(d.Id))
                        {
                            g.DrawString(core.Point[d.Id].ToString(), fontW, Brushes.Azure,
                                x * blockWidth + i * 128 + 20, y * blockHeight + j * 128 + 20);
                            g.DrawString(core.Point[d.Id].ToString(), font, Brushes.Aqua,
                                x * blockWidth + i * 128 + 28, y * blockHeight + j * 128 + 28);

                        }
                    }
                }
            }

            b++;
        }

        bitmap.Save(file);
    }
}