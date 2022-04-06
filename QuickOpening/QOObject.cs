using Newtonsoft.Json;

namespace QuickOpening
{
    public class QOObject
    {
        [JsonProperty]
        public string pathObj { get; private set; }

        [JsonProperty]
        public bool isFolder { get; private set; }

        [JsonProperty]
        public string pathImage { get; private set; }

        [JsonConstructor]
        public QOObject(string pathObj, bool isFolder, string pathImage)
        {
            this.pathObj = pathObj;
            this.isFolder = isFolder;
            this.pathImage = pathImage;
        }

        public QOObject(QOObject obj)
        {
            this.pathObj = obj.pathObj;
            this.isFolder = obj.isFolder;
            this.pathImage = obj.pathImage;
        }
        public QOObject() { }
    }
}
