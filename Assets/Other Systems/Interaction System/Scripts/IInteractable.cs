namespace InteractionSystem
{
    public interface IInteractable
    {
        public void GetInteracted();
        public void ToggleVisualCue(bool activate);
    }
}