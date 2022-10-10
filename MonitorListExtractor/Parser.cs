using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MonitorListExtractor
{
    public class Parser
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private XmlDocument doc;

        public Parser(string filePath)
        {
            doc = new XmlDocument();
            doc.Load(filePath);
        }

        public void Parse()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (XmlNode configurationsList in doc.DocumentElement.ChildNodes)
            {
                foreach (XmlNode configuration in configurationsList.ChildNodes)
                {
                    // Get the initial information
                    string name = configuration.Attributes["name"].InnerText;
                    string description = configuration.Attributes["description"].InnerText;
                    bool mechanicalReels = false;
                    int monitors = 0;
                    foreach (XmlNode configurationNodes in configuration.ChildNodes)
                    {
                        // Find the number of monitors
                        if (configurationNodes.Name.Contains("Monitors"))
                        {
                            monitors = configurationNodes.ChildNodes.Count;
                        }
                        // Check if mechReels are included
                        if (configurationNodes.Name.Contains("MechanicalReels"))
                        {
                            mechanicalReels = true;
                        }
                    }
                    // Compile the collected info for this configuration
                    string configurationInfo = $"{name}\n- {description}\n- {monitors} monitors";
                    if (mechanicalReels)
                    {
                        configurationInfo += " + MechReels";
                    }
                    // Add collected info to the stringbuilder
                    stringBuilder.AppendLine(configurationInfo);
                }
            }
            // Log all collected monitor info
            log.Info(stringBuilder.ToString());
        }
    }
}
