namespace MVE.Admin.ViewModels
{
    public class MenuViewModel
    {
        public int Id { get; set; }
        public int ParentId { get; set; }

        public long PermissionId { get; set; }
        public string MenuName { get; set; }
        public string SubMenuName { get; set; }

        public string MenuIcon { get; set; }
        public string SubMenuIcon { get; set; }
        public string MenuUrl { get; set; }
        public bool IsActive { get; set; }
        public static List<MenuViewModel> MenuItems { get; set; }
        public static List<MenuViewModel> AdminMenuItems { get; set; }

    }
}
