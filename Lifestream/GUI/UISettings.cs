using Dalamud.Interface.Components;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lifestream.GUI
{
    internal static class UISettings
    {
        internal static void Draw()
        {
            ImGui.Checkbox("启用悬浮窗", ref P.Config.Enable);
            if (P.Config.Enable)
            {
                ImGui.Checkbox($"显示都市传送网菜单", ref P.Config.ShowAethernet);
                ImGui.Checkbox($"显示跨界传送菜单", ref P.Config.ShowWorldVisit);
                ImGui.Checkbox("在地图上点击城内以太之晶以传送", ref P.Config.UseMapTeleport);
                ImGui.Checkbox($"减慢城内以太之晶传送速度", ref P.Config.SlowTeleport);
                if (P.Config.SlowTeleport)
                {
                    ImGui.SetNextItemWidth(200f);
                    ImGui.DragInt("传送延迟（毫秒）", ref P.Config.SlowTeleportThrottle);
                }
                ImGui.Checkbox("固定悬浮窗位置", ref P.Config.FixedPosition);
                if (P.Config.FixedPosition)
                {
                    ImGui.SetNextItemWidth(200f);
                    ImGuiEx.EnumCombo("水平基础位置", ref P.Config.PosHorizontal);
                    ImGui.SetNextItemWidth(200f);
                    ImGuiEx.EnumCombo("垂直基础位置", ref P.Config.PosVertical);
                    ImGui.SetNextItemWidth(200f);
                    ImGui.DragFloat2("偏移", ref P.Config.Offset);
                }
                ImGui.SetNextItemWidth(100f);
                ImGui.InputInt("按钮宽度修正值", ref P.Config.ButtonWidth);
                ImGui.SetNextItemWidth(100f);
                ImGui.InputInt("城内以太之晶按钮高度修正值", ref P.Config.ButtonHeightAetheryte);
                ImGui.SetNextItemWidth(100f);
                ImGui.InputInt("跨界传送按钮高度修正值", ref P.Config.ButtonHeightWorld);
                //ImGui.Checkbox($"Allow closing Lifestream with ESC", ref P.Config.AllowClosingESC2);
                //ImGuiComponents.HelpMarker("To reopen, reenter the proximity of aetheryte");
                ImGui.Checkbox($"当打开游戏UI时隐藏悬浮窗", ref P.Config.HideAddon);
                ImGui.SetNextItemWidth(200f);
                ImGuiEx.EnumCombo($"跨界传送点", ref P.Config.WorldChangeAetheryte, Lang.WorldChangeAetherytes);
                ImGui.Checkbox($"在较远距离的跨界传送时尽可能走到附近的城内以太之晶（存疑）", ref P.Config.WalkToAetheryte);
                ImGui.Checkbox($"在伊修加德基础层以太之光的悬浮窗中添加天穹街", ref P.Config.Firmament);
                ImGui.Checkbox($"在跨界传送时自动离开非跨服小队", ref P.Config.LeavePartyBeforeWorldChange);
                ImGui.Checkbox($"允许跨大区", ref P.Config.AllowDcTransfer);
                ImGui.Checkbox($"跨大区前离开小队", ref P.Config.LeavePartyBeforeLogout);
                ImGui.Checkbox($"若不在休息区跨大区，先传送至跨界传送点", ref P.Config.TeleportToGatewayBeforeLogout);
                ImGui.Checkbox($"在跨大区后传送至跨界传送点", ref P.Config.DCReturnToGateway);
                ImGui.Checkbox($"在跨界传送/跨大区后传送至特定的都市传送网目的地", ref P.Config.WorldVisitTPToAethernet);
                if (P.Config.WorldVisitTPToAethernet)
                {
                    ImGui.SetNextItemWidth(250f);
                    ImGui.InputText("都市传送网目的地，就像你在“/li”命令中使用的一样", ref P.Config.WorldVisitTPTarget, 50);
                    ImGui.Checkbox($"仅接受命令传送，禁止悬浮窗传送", ref P.Config.WorldVisitTPOnlyCmd);
                }
                ImGui.Checkbox($"隐藏进度条", ref P.Config.NoProgressBar);
            }
            if (ImGui.CollapsingHeader("已隐藏的城内以太之晶"))
            {
                uint toRem = 0;
                foreach (var x in P.Config.Hidden)
                {
                    ImGuiEx.Text($"{Svc.Data.GetExcelSheet<Aetheryte>().GetRow(x)?.AethernetName.Value?.Name.ToString() ?? x.ToString()}");
                    ImGui.SameLine();
                    if (ImGui.SmallButton($"删除##{x}"))
                    {
                        toRem = x;
                    }
                }
                if (toRem > 0)
                {
                    P.Config.Hidden.Remove(toRem);
                }
            }
        }
    }
}
