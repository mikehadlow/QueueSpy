namespace QueueSpy.Messages
{
	public class ChangePassword
	{
		public int UserId { get; set; }
		public string NewPassword { get; set; }
	}
}

