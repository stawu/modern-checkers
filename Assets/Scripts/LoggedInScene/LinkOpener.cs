using UnityEngine;

namespace LoggedInScene
{
    public class LinkOpener : MonoBehaviour
    {
        public void OpenLink(string link)
        {
            Application.OpenURL(link);
        }
    }
}
