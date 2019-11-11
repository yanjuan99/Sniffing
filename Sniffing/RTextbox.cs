namespace Sniffing
{
    //public class RTextbox
    //{
    //    private Mutex load_mte = new Mutex();

    //    private void Setcolor(RichTextBox textbox, TextPointer s, TextPointer e, Brush colors)
    //    {
    //        textbox.Dispatcher.Invoke(() =>
    //        {
    //            TextRange range = textbox.Selection;
    //            range.Select(s, e);
    //            range.ApplyPropertyValue(TextElement.ForegroundProperty, colors);
    //            range.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
    //        });
    //    }

    //    private int Sotext(string a, ref int k, ref Brush colors)
    //    {
    //        List<string> keyword_list = new List<string> { "Send", "Recv" };
    //        int s = a.IndexOf(keyword_list[0], 0);
    //        int r = a.IndexOf(keyword_list[1], 0);
    //        if (s == -1 && r == -1)
    //        {
    //            return -1;
    //        }
    //        else if (s == -1)
    //        {
    //            k = keyword_list[1].Length; colors = Brushes.Purple;
    //            return r;
    //        }
    //        else if (r == -1)
    //        {
    //            k = keyword_list[0].Length; colors = Brushes.Blue;
    //            return s;
    //        }
    //        else if (s > r)
    //        {
    //            k = keyword_list[1].Length; colors = Brushes.Purple;
    //            return r;
    //        }
    //        else
    //        {
    //            k = keyword_list[0].Length; colors = Brushes.Blue;
    //            return s;
    //        }
    //    }

    //    private int add_text(RichTextBox textbox)
    //    {
    //        TextPointer position = textbox.Document.ContentEnd;
    //        while (true)
    //        {
    //            if (position.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.Text)
    //            {
    //                position = position.GetNextContextPosition(LogicalDirection.Backward);

    //                string text = position.GetTextInRun(LogicalDirection.Forward);

    //                for (int c = 0; ;)
    //                {
    //                    int len = 0; Brush colors = Brushes.Black;
    //                    c = Sotext(text, ref len, ref colors);
    //                    //c = text.IndexOf(keyword, c);
    //                    if (c == -1)
    //                        return 0;
    //                    TextPointer start = position.GetPositionAtOffset(c);
    //                    TextPointer end = position.GetPositionAtOffset(c + len);

    //                    /*
    //                        aaaabbbbcccceeeeSendffff
    //                        c==16
    //                        position.GetPositionAtOffset(-c)
    //                        position.GetPositionAtOffset(-keyword.Length)
    //                        aaaabbbbcccceeeeSendffff
    //                            ****
    //                       -4 <-- -c
    //                     */

    //                    TextRange range = textbox.Selection;

    //                    range.Select(start, end);
    //                    range.ApplyPropertyValue(TextElement.ForegroundProperty, colors);
    //                    range.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);

    //                    return c += len;
    //                }
    //            }
    //            position = position.GetNextContextPosition(LogicalDirection.Backward);
    //        }
    //    }

    //    private void put_text(RichTextBox textbox, string str)
    //    {
    //        textbox.Dispatcher.Invoke(() =>
    //        {
    //            textbox.AppendText(str);

    //            while (true)
    //            {
    //                if (add_text(textbox) == 0)
    //                {
    //                    return;
    //                }

    //            }
    //        });
    //    }

    //    private void ReadText(RichTextBox textbox, string path)
    //    {
    //        try
    //        {
    //            using (FileStream stream = new FileStream(path, FileMode.Open))
    //            {
    //                using (StreamReader reader = new StreamReader(stream, Encoding.Default))
    //                {
    //                    while (!reader.EndOfStream)
    //                    {
    //                        var content = reader.ReadLine();
    //                        if (!string.IsNullOrEmpty(content))
    //                        {
    //                            put_text(textbox, content + "\n");
    //                        }
    //                        //  System.Threading.Thread.Sleep(5);
    //                    }
    //                }
    //            }
    //        }
    //        catch
    //        {
    //        }
    //    }

    //    public struct MyPointer
    //    {
    //        public TextPointer s;
    //        public TextPointer e;
    //        public Brush color;
    //    }
    //    List<MyPointer> myPointer = new List<MyPointer>();
    //    public void replace_all(RichTextBox textbox, string str, Brush colors)
    //    {
    //        TextPointer position = textbox.Document.ContentStart;
    //        while (position != null)
    //        {
    //            if (position.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
    //            {
    //                string text = position.GetTextInRun(LogicalDirection.Forward);
    //                int index = 0;
    //                while (index < text.Length)
    //                {
    //                    index = text.IndexOf(str, index);
    //                    if (index == -1)
    //                    {
    //                        break;
    //                    }
    //                    TextPointer start = position.GetPositionAtOffset(index);
    //                    TextPointer end = start.GetPositionAtOffset(str.Length);

    //                    myPointer.Add(new MyPointer { s = start, e = end, color = colors });
    //                    index += str.Length;
    //                }
    //            }
    //            position = position.GetNextContextPosition(LogicalDirection.Forward);
    //        }

    //        Console.WriteLine("搜索完成: " + str + " " + myPointer.Count.ToString());
    //    }
    //    public void replace(RichTextBox textbox)
    //    {
    //        Console.WriteLine("替换start" + Thread.CurrentThread.ManagedThreadId.ToString());
    //        textbox.Dispatcher.Invoke(() =>
    //        {
    //            foreach (var item in myPointer)
    //            {
    //                //TextRange range = textbox.Selection;
    //                textbox.Selection.Select(item.s, item.e);
    //                textbox.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, item.color);
    //                textbox.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
    //            }
    //        });
    //        myPointer.Clear();
    //        Console.WriteLine("替换enf");
    //    }
    //    public void LoadFile(RichTextBox richtext, string filename)
    //    {
    //        if (string.IsNullOrEmpty(filename))
    //        {
    //            throw new ArgumentNullException();
    //        }
    //        if (!File.Exists(filename))
    //        {
    //            throw new FileNotFoundException();
    //        }

    //        using (FileStream stream = File.OpenRead(filename))
    //        {
    //            StreamReader sr = new StreamReader(stream, Encoding.Default);
    //            var read_stream = new MemoryStream(Encoding.UTF8.GetBytes(sr.ReadToEnd()));
    //            richtext.Dispatcher.Invoke(() =>
    //            {
    //                TextRange documentTextRange = new TextRange(richtext.Document.ContentStart, richtext.Document.ContentEnd);
    //                documentTextRange.Load(read_stream, DataFormats.Text);
    //            });
    //        }
    //        Console.WriteLine("文本加载完毕");
    //    }
    //}
}