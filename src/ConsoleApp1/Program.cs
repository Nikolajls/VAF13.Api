using ConsoleApp1.Helpers;

namespace ConsoleApp1
{
  internal class Program
  {
    static void Main(string[] args)
    {
      Console.WriteLine("Hello, World!");
      IntPtr membersHandle = NativeMethods.FindWindow(null, "Members");
      //var x = NativeMethods.FindWindowLine
      IntPtr firstNameHandle1 = NativeMethods.FindWindowEx(membersHandle, IntPtr.Zero, "ThunderRT6TextBox", null);
      IntPtr firstNameHandle2 = NativeMethods.FindWindowEx(membersHandle, IntPtr.Zero, "ThunderRT6TextBox", "Members");
      IntPtr firstNameHandle3 = NativeMethods.FindWindowEx(membersHandle, IntPtr.Zero, "ThunderRT6TextBox", "");
      IntPtr d = FindWindowByIndex(membersHandle, 3);

      Console.WriteLine(membersHandle.ToString());
      Console.WriteLine(membersHandle);
      Console.WriteLine(firstNameHandle1.ToString());
      Console.WriteLine(firstNameHandle1);
      Console.WriteLine(firstNameHandle2.ToString());
      Console.WriteLine(firstNameHandle2);
      Console.WriteLine(firstNameHandle3.ToString());
      Console.WriteLine(firstNameHandle3);


    }



    static IntPtr FindWindowByIndex(IntPtr hWndParent, int index)
    {
      if (index == 0)
        return hWndParent;
      else
      {
        int ct = 0;
        IntPtr result = IntPtr.Zero;
        do
        {
          result = NativeMethods.FindWindowEx(hWndParent, result, "Button", null);
          if (result != IntPtr.Zero)
            ++ct;
        }
        while (ct < index && result != IntPtr.Zero);
        return result;
      }
    }
  }

}