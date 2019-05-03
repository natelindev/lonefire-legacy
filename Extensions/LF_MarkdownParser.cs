using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Westwind.AspNetCore.Markdown;

namespace lonefire.Extensions
{
    public static class LF_MarkdownParser
    {
    
        public static string Parse(string markdown, string img_path = "")
        {
            //Markdown pre parse
            string html = Markdown.Parse(markdown);
            string newhtml = "";

            //Find images
            string pattern = @"<img[^>]+>";
            var img_tags = Regex.Matches(html, pattern);


            if (img_tags != null & img_tags.Count > 0)
            {
            
                List<int> indexs = new List<int>();
                List<string> new_imgs = new List<string>();
                foreach (Match img_tag in img_tags)
                {
                    //Collect the indices
                    indexs.Add(img_tag.Index);
                    indexs.Add(img_tag.Index+img_tag.Length);
                }

                List<string> Parts = html.SplitAt(indexs.ToArray()).ToList();

                //Add Path & class
                foreach (Match img_tag in img_tags)
                {
                    var src_regex = @"src=\""[^""]+\""";
                    var src_tags = Regex.Matches(img_tag.Value, src_regex);
                    var src_tag = src_tags[0];
                    new_imgs.Add(img_tag.Value.Substring(0, src_tag.Index) + "class=\"img-fluid img-center mb-3 shadow-lg\" " + "src=\"" + img_path + src_tag.Value.Substring(5, src_tag.Length - 5) + img_tag.Value.Substring(src_tag.Index+src_tag.Length,img_tag.Length- src_tag.Index - src_tag.Length));
                }

                //Replace
                int ctr = 0;
                for(int i = 0; i < Parts.Count; ++i)
                {
                    if(Parts[i].Contains("<img"))
                    {
                        Parts[i] = new_imgs[ctr++];
                    }
                }

                //Rebuild
                newhtml = string.Join("",Parts);
            }
            else
            {
                newhtml = html;
            }

            //only work with style do not try to use it on other html elements
            newhtml = RemoveByRegex(newhtml, @"<style>[\s\S]+<\/style>");

            return newhtml;
        }

        public static string ParseAsPlainText(string markdown)
        {
            return Regex.Replace(Markdown.Parse(markdown), "<.*?>", string.Empty);
        }

        public static IEnumerable<string> SplitAt(this string source, params int[] index)
        {
            var indices = new[] { 0 }.Union(index).Union(new[] { source.Length });

            return indices
                        .Zip(indices.Skip(1), (a, b) => (a, b))
                        .Select(_ => source.Substring(_.a, _.b - _.a));
        }

        public static string ParseWithoutStyle(string markdown)
        {
            //only work with style do not try to use it on other html elements
            return Regex.Replace(Markdown.Parse(markdown,false,false,sanitizeHtml:true), @"<style>[\s\S]+<\/style>", string.Empty);
        }

        public static string RemoveByRegex(string oldHtml,string regex)
        {
            return Regex.Replace(oldHtml, regex, string.Empty);
        }
    }
}
