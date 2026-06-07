namespace SunderedCrown.Core
{
    /// <summary>
    /// The set of save slots and which one is "active" (the slot the campaign director autosaves to and
    /// Continue loads from). The main menu / slot manager set <see cref="Active"/> before launching the
    /// campaign; everything else just reads it. "save" stays the default/autosave slot for back-compat.
    /// </summary>
    public static class SaveSlots
    {
        public static string Active = "save";

        public static readonly string[] All = { "save", "slot1", "slot2", "slot3" };

        public static string Label(string slot) => slot == "save"
            ? "Autosave"
            : "Slot " + (slot.Length > 4 ? slot.Substring(4) : slot);
    }
}
