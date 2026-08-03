namespace Himam_main.Helpers;

public static class UserDisplayHelper
{
    private static readonly Dictionary<string, (string Ar, string BadgeClass)> RoleMap = new()
    {
        ["Super Admin"] = ("المدير الأعلى", "role-super"),
        ["Site Manager"] = ("مدير الموقع", "role-manager"),
        ["Content Editor"] = ("محرر المحتوى", "role-editor"),
        ["Customer Service"] = ("خدمة العملاء", "role-support")
    };

    public static (string Ar, string BadgeClass) GetRoleDisplay(string roleName) =>
        RoleMap.TryGetValue(roleName, out var display) ? display : (roleName, "role-support");

    public static string GetInitials(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return "؟";

        var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length >= 2)
            return string.Concat(parts[0][..1], parts[1][..1]);

        return name.Length >= 2 ? name[..2] : name;
    }

    public static string FormatRelativeDate(DateTime? date)
    {
        if (date == null)
            return "—";

        return date.Value.ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("ar-SA"));
    }
}
