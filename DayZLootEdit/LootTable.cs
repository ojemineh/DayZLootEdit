using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Linq;

namespace DayZLootEdit
{
    class LootTable
    {
        public string FilePath { get; private set; }
        public XElement XML { get; private set; }

        public ObservableCollection<LootType> Loot { get; set; } = new ObservableCollection<LootType>();

        public LootTable(string lFile)
        {
            FilePath = lFile;
        }

        public void LoadFile(string lFile = "")
        {
            if (lFile != "") { FilePath = lFile; }
            XML = XElement.Load(FilePath);
            process();
        }

        public void SaveFile(string lFile = "")
        {
            if (lFile != "") { FilePath = lFile; }
            FileInfo file = new FileInfo(FilePath);
            if (file.Exists)
            {
                DateTime now = DateTime.Now;
                FileInfo backup = new FileInfo(file.FullName + ".backup-" + now.ToString("yyyyMMdd-HHmmss") + ".xml");
                if (!backup.Exists)
                {
                    file.CopyTo(backup.FullName);
                }
            }
            XML.Save(FilePath);
        }

        private void process()
        {
            Loot.Clear();
            foreach(XElement node in XML.Elements())
            {
                Loot.Add(new LootType(node));
            }
        }

    }
}
