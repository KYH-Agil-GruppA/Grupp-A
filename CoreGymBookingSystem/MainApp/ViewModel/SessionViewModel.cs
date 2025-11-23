namespace MainApp.ViewModel
{
    public class SessionViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Summary of the activities.
        /// </summary>
        public string Description { get; set; } = string.Empty;


        /// <summary>
        /// Summary of the activities.
        /// </summary>
        public string Category { get; set; } = string.Empty;
    }
}
