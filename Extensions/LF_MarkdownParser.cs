using System;
using System.Collections.Generic;
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
                List<string> oldParts = new List<string>();
                List<string> newParts = new List<string>();
                for (int i = 0; i < img_tags.Count; ++i)
                {
                    if (oldParts.Count == 0)
                    {
                        //first one
                        oldParts.Add(html.Substring(0, img_tags[i].Index - 0));
                    }
                    else
                    {
                        oldParts.Add(html.Substring(img_tags[i - 1].Index + img_tags[i - 1].Value.Length, img_tags[i].Index - img_tags[i - 1].Index - img_tags[i - 1].Value.Length));
                    }
                }
                //last one
                oldParts.Add(html.Substring(img_tags[img_tags.Count - 1].Index + img_tags[img_tags.Count - 1].Value.Length, html.Length - img_tags[img_tags.Count - 1].Index - img_tags[img_tags.Count - 1].Value.Length));

                foreach (Match img_tag in img_tags)
                {
                    //Add Path & class
                    var src_regex = @"src=\""[^""]+\""";
                    var src_tags = Regex.Matches(img_tag.Value, src_regex);
                    var src_tag = src_tags[0];
                    newParts.Add(img_tag.Value.Substring(0, src_tag.Index) + "class=\"img-fluid mx-auto animated--shadow-lg\" " + "src=\"" + img_path + src_tag.Value.Substring(5, src_tag.Length - 5) + img_tag.Value.Substring(src_tag.Index+src_tag.Length,img_tag.Length- src_tag.Index - src_tag.Length));
                }

                //Rebuild
                for (int i = 0; i < img_tags.Count - 1; ++i)
                {
                    newhtml += oldParts[i];
                    newhtml += newParts[i];
                    Console.WriteLine(newParts[i]);
                }
                newhtml += oldParts[oldParts.Count - 1];
            }

            return newhtml;
        }
    }
}
