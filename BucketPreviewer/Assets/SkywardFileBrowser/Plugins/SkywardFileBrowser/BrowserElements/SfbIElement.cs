namespace SkywardRay.FileBrowser {
	public interface SfbIElement {
		void Init (SfbInternal fileBrowser);

		void SetFocus ();

		void RecieveMessage (string message);
	}
}