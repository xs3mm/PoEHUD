using PoeHUD.Poe.Components;
using System;

namespace PoeHUD.Poe.Elements
{
    public class InventoryItemIcon : Element
    {
        private readonly Func<Element> inventoryItemTooltip;
        private readonly Func<Element> itemInChatTooltip;
        private readonly Func<ItemOnGroundTooltip> toolTipOnground;
        private ToolTipType? toolTip;

        public InventoryItemIcon()
        {
            toolTipOnground = () => Game.IngameState.IngameUi.ItemOnGroundTooltip;
            inventoryItemTooltip = () => ReadObject<Element>(Address + 0xA74);
            itemInChatTooltip = () => ReadObject<Element>(Address + 0x800);
        }

        public ToolTipType ToolTipType => (ToolTipType)(toolTip ?? (toolTip = GetToolTipType()));

        public Element Tooltip
        {
            get
            {
                switch (ToolTipType)
                {
                    case ToolTipType.ItemOnGround:
                        return toolTipOnground().Tooltip;

                    case ToolTipType.InventoryItem:
                        return inventoryItemTooltip();
                    case ToolTipType.ItemInChat:
                        return itemInChatTooltip();
                }
                return null;
            }
        }

        public Element ItemFrame
        {
            get
            {
                switch (ToolTipType)
                {
                    case ToolTipType.ItemOnGround:
                        return toolTipOnground().ItemFrame;
                    default:
                        return null;
                }
            }
        }

        public Entity Item
        {
            get
            {
                switch (ToolTipType)
                {
                    case ToolTipType.ItemOnGround:

                        ItemsOnGroundLabelElement le = Game.IngameState.IngameUi.ReadObjectAt<ItemsOnGroundLabelElement>(0xA68);
                        if (le == null)
                            return null;
                        Entity e = le.ReadObjectAt<Entity>(OffsetBuffers + 0x1D4);
                        if (e == null)
                            return null;
                        return e.GetComponent<WorldItem>().ItemEntity;

                    case ToolTipType.InventoryItem:
                        return ReadObject<Entity>(Address + 0xA94);
                }
                return null;
            }
        }

        private ToolTipType GetToolTipType()
        {
            if (inventoryItemTooltip() != null && inventoryItemTooltip().IsVisible)
            {
                return ToolTipType.InventoryItem;
            }
            if (toolTipOnground != null && toolTipOnground().Tooltip != null && toolTipOnground().TooltipUI != null && toolTipOnground().TooltipUI.IsVisible)
            {
                return ToolTipType.ItemOnGround;
            }
            return ToolTipType.None;
        }
    }

    public enum ToolTipType
    {
        None,
        InventoryItem,
        ItemOnGround,
        ItemInChat
    }
}