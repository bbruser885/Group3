using LiteDB;

namespace ChocAn
{
    public class Manager : Provider
    {
        // `Manager.Collection` is a LiteDB collection of managers
        public new static LiteCollection<Manager> Collection = DB.GetCollection<Manager>("managers");
    }
}
