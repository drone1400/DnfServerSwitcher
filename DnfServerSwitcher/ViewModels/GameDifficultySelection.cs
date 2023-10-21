namespace DnfServerSwitcher.ViewModels {
    public class GameDifficultySelection {
        public string DisplayName { get; }
        public int Value { get; }

        public GameDifficultySelection(string displayName, int value) {
            this.DisplayName = displayName;
            this.Value = value;
        }
    }
}
