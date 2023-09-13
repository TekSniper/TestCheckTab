using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace TestCheckTab.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public List<Menu> menus { get; set; }
        public string WarningMsg { get; set; } = string.Empty;
        IConfigurationRoot GetConfiguration()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            return builder.Build();
        }

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }
        public List<Menu> GetMenus()
        {
            var connectionString = GetConfiguration().GetSection("ConnectionStrings").GetSection("K@zy$").Value!;
            var items = new List<Menu>();
            using (var cnx = new SqlConnection(connectionString))
            {
                cnx.Open();
                var cm = new SqlCommand("select * from Menu where parent_menu is not null", cnx);
                var reader = cm.ExecuteReader();
                while (reader.Read())
                {
                    var menu = new Menu();
                    menu.Code = reader.GetString(0);
                    menu.Designation = reader.GetString(1);
                    menu.Url = reader.GetString(3);
                    menu.Icone = reader.GetString(4);
                    items.Add(menu);
                }
            }

            return items;
        }

        public void OnGet()
        {
            this.menus = GetMenus();
        }
        public void OnPostSubmit(string[] menu)
        {
            this.menus = GetMenus();
            string msg = "Les items : \n";
            foreach(Menu item in  this.menus)
            {
                if (menu.Contains(item.Code))
                {
                    item.Checked = true;
                    msg += string.Format("{0}\n", item.Code);
                }
            }
            ViewData["msg"] = msg;
        }
    }
    public class Menu
    {
        public string Code { get; set; }
        public string Designation { get; set; }
        public string Url { get; set; }
        public string Icone { get; set; }
        public bool Checked { get; set; } = false;
    }
}