using Core.Entities;
using Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class EmailBodyBuilder : IEmailBodyBuilder
    {


        public string GenerateEmailBody(string template, Dictionary<string, string> placeholders)
        {

            var filePath = $"{Directory.GetCurrentDirectory()}\\Templates\\{template}.html";
            var str = new StreamReader(filePath);
            var templateContent = str.ReadToEnd();
            str.Close();

            foreach (var placeholder in placeholders)
                templateContent = templateContent.Replace($"[{placeholder.Key}]", placeholder.Value);

            return templateContent;

        }
    }
}
