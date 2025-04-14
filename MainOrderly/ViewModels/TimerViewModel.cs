namespace MainOrderly.WebApp.ViewModels
{
    public class TimerViewModel
    {
        public DateTime StartTime { get; set; }
        
        public TimerViewModel()
        {
            this.StartTime = DateTime.Now;  
        }
    }
}
