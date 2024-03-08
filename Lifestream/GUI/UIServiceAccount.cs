using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lifestream.GUI
{
    internal static class UIServiceAccount
    {
        internal static void Draw()
        {
            ImGuiEx.TextWrapped($"如果你拥有一个以上的服务账号，你必须将每个角色分配给正确的服务账号。\n请登录以将角色添加到此列表中。");
            ImGui.Checkbox($"从AutoRetainer获取服务账号数据", ref P.Config.UseAutoRetainerAccounts);
            List<string> ManagedByAR = [];
            if (P.AutoRetainerApi?.Ready == true && P.Config.UseAutoRetainerAccounts)
            {
                var chars = P.AutoRetainerApi.GetRegisteredCharacters();
                foreach (var c in chars)
                {
                    var data = P.AutoRetainerApi.GetOfflineCharacterData(c);
                    if (data != null)
                    {
                        var name = $"{data.Name}@{data.World}";
                        ManagedByAR.Add(name);
                        ImGui.SetNextItemWidth(150f);
                        if (ImGui.BeginCombo($"{name}", data.ServiceAccount == -1 ? "未选择" : $"服务账号 {data.ServiceAccount+1}"))
                        {
                            for (int i = 0; i < 10; i++)
                            {
                                if (ImGui.Selectable($"服务账号 {i + 1}"))
                                {
                                    P.Config.ServiceAccounts[name] = i;
                                    data.ServiceAccount = i;
                                    P.AutoRetainerApi.WriteOfflineCharacterData(data);
                                    Notify.Info($"设置已保存至AutoRetainer");
                                }
                            }
                            ImGui.EndCombo();
                        }
                        ImGui.SameLine();
                        ImGuiEx.Text(ImGuiColors.DalamudRed, $"由AutoRetainer管理");
                    }
                }
            }
            foreach (var x in P.Config.ServiceAccounts)
            {
                if (ManagedByAR.Contains(x.Key)) continue;
                ImGui.SetNextItemWidth(150f);
                if (ImGui.BeginCombo($"{x.Key}", x.Value==-1?"未选择":$"服务账号 {x.Value+1}"))
                {
                    for (int i = 0; i < 10; i++)
                    {
                        if(ImGui.Selectable($"服务账号 {i+1}")) P.Config.ServiceAccounts[x.Key] = i;
                    }
                    ImGui.EndCombo();
                }
                ImGui.SameLine();
                if (ImGui.Button("删除"))
                {
                    new TickScheduler(() => P.Config.ServiceAccounts.Remove(x.Key));
                }
            }
        }
    }
}
