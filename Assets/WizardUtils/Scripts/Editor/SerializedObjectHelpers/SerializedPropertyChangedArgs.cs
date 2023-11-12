namespace WizardUtils.SerializedObjectHelpers
{
    public class SerializedPropertyChangedArgs<T>
    {
        public T OldValue;
        public T NewValue;

        public SerializedPropertyChangedArgs(T oldValue, T newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
