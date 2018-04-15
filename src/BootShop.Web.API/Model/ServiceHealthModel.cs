namespace BootShop.Web.API.Model
{
    public class ServiceHealthModel
    {
        public ServiceHealthModel(string name, bool up)
        {
            Name = name;
            Up = up;
        }

        public string Name { get; set; }
        public bool Up { get; set; }
    }
}