/*
 * Developer    : Willy Kimura (WK).
 * Library      : BetterFolderBrowser.
 * License      : MIT.
 * 
 * This .NET component was written to help developers
 * provide a better folder-browsing and selection
 * experience to users by employing the old Windows
 * Vista folder browser dialog in place of the current
 * 'FolderBrowserDialog' tree-view style. This dialog
 * implementation mimics the 'OpenFileDialog' design
 * which allows for a much easier viewing, selection, 
 * and search experience using Windows Explorer.
 * 
 * +------------------------------------+
 * | if (BetterFolderBrowserIsActive()) |
 * | --> AwesomeUX = true;              |
 * +------------------------------------+
 * 
 * Improvements are always welcome :)
 * 
 */


using System;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.ComponentModel.Design;
using System.Reflection;

using WK.Libraries.BetterFolderBrowserNS.Editors;
using WK.Libraries.BetterFolderBrowserNS.Helpers;

namespace WK.Libraries.BetterFolderBrowserNS
{
    partial class BetterFolderBrowser
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }

        #endregion
    }

    /// <summary>
    /// A Windows Forms component that enhances the standard folder-browsing experience.
    /// </summary>
    [DefaultProperty("RootFolder")]
    [ToolboxBitmap(typeof(FolderBrowserDialog))]
    [Description("A .NET component library that delivers a better folder-browsing and selection experience.")]
    public partial class BetterFolderBrowser : CommonDialog
    {
        #region Constructors

        public BetterFolderBrowser()
        {
            InitializeComponent();

            SetDefaults();
        }

        public BetterFolderBrowser(IContainer container)
        {
            container.Add(this);

            InitializeComponent();

            SetDefaults();
        }

        #endregion

        #region Fields

        private Helpers.BetterFolderBrowserDialog _dialog =
            new Helpers.BetterFolderBrowserDialog();

        /// <summary>
        /// Used in creating a <see cref="UITypeEditor"/> service
        /// for extending its usage into the Properties window.
        /// Developers can use it where possible.
        /// </summary>
        internal IWindowsFormsEditorService editorService;

        #endregion

        #region Properties

        #region Browsable

        /// <summary>
        /// Gets or sets the folder dialog box title.
        /// </summary>
        [Category("Better Folder Browsing")]
        [Description("Sets the folder dialog box title.")]
        public string Title
        {
            get { return _dialog.Title; }
            set { _dialog.Title = value; }
        }

        /// <summary>
        /// Gets or sets the root folder where the browsing starts from.
        /// </summary>
        [Category("Better Folder Browsing")]
        [Editor(typeof(SelectedPathEditor), typeof(UITypeEditor))]
        [Description("Sets the root folder where the browsing starts from.")]
        public string RootFolder
        {
            get { return _dialog.InitialDirectory; }
            set { _dialog.InitialDirectory = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the 
        /// dialog box allows multiple folders to be selected.
        /// </summary>
        [Category("Better Folder Browsing")]
        [Description("Sets a value indicating whether the dialog " +
                     "box allows multiple folders to be selected.")]
        public bool Multiselect
        {
            get { return _dialog.AllowMultiselect; }
            set { _dialog.AllowMultiselect = value; }
        }

        #endregion

        #region Non-browsable

        /// <summary>
        /// Gets the folder-path selected by the user.
        /// </summary>
        [Browsable(false)]
        public string SelectedPath
        {
            get { return _dialog.FileName; }
        }

        /// <summary>
        /// Gets the list of folder-paths selected by the user.
        /// </summary>
        [Browsable(false)]
        public string[] SelectedPaths
        {
            get { return _dialog.FileNames; }
        }

        /// <summary>
        /// Variant of <see cref="SelectedPath"/> property.
        /// Gets the folder-path selected by the user.
        /// </summary>
        [Browsable(false)]
        public string SelectedFolder
        {
            get { return _dialog.FileName; }
        }

        /// <summary>
        /// Variant of <see cref="SelectedPaths"/> property.
        /// Gets the list of folder-paths selected by the user.
        /// </summary>
        [Browsable(false)]
        public string[] SelectedFolders
        {
            get { return _dialog.FileNames; }
        }

        #endregion

        #endregion

        #region Methods

        #region Private

        private void SetDefaults()
        {
            _dialog.AllowMultiselect = false;
            _dialog.Title = "Please select a folder...";
            _dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        }

        #endregion

        #region Public

        /// <summary>
        /// Runs a common dialog box with a default owner.
        /// </summary>
        public new DialogResult ShowDialog()
        {
            DialogResult result = DialogResult.Cancel;

            if (_dialog.ShowDialog(IntPtr.Zero))
                result = DialogResult.OK;
            else
                result = DialogResult.Cancel;

            return result;
        }

        /// <summary>
        /// Runs a common dialog box with the specified owner.
        /// </summary>
        /// <param name="owner">
        /// Any object that implements <see cref="IWin32Window"/> that represents
        /// the top-level window that will own the modal dialog box.
        /// </param>
        public new DialogResult ShowDialog(IWin32Window owner)
        {
            DialogResult result = DialogResult.Cancel;

            if (_dialog.ShowDialog(owner.Handle))
                result = DialogResult.OK;
            else
                result = DialogResult.Cancel;

            return result;
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Specifies a common dialog box.
        /// </summary>
        /// <param name="hwndOwner">
        /// Any object that implements <see cref="IWin32Window"/> that represents
        /// the top-level window that will own the modal dialog box.
        /// </param>
        /// <returns></returns>
        protected override bool RunDialog(IntPtr hwndOwner)
        {
            return _dialog.ShowDialog(hwndOwner);
        }

        /// <summary>
        /// Resets all properties to their default values.
        /// </summary>
        public override void Reset()
        {
            SetDefaults();
        }

        #endregion

        #endregion
    }
}

namespace WK.Libraries.BetterFolderBrowserNS.Editors
{
    /// <summary>
    /// Provides a custom <see cref="BetterFolderBrowser"/> UI Editor
    /// for browsing through folders via the Properties window. 
    /// This allows for the selection of a single folder.
    /// It's designed as a replacement for <see cref="FolderBrowserDialog"/>'s
    /// <see cref="UITypeEditor"/>.
    /// 
    /// <para>
    /// Example:
    /// <code>[Editor(typeof(BetterFolderBrowserPathEditor), typeof(UITypeEditor))]</code>
    /// </para>
    /// </summary>
    [DebuggerStepThrough]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class SelectedPathEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context,
                                IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService editorService =
               provider.GetService(typeof(IWindowsFormsEditorService)) as
               IWindowsFormsEditorService;

            if (editorService != null)
            {
                BetterFolderBrowser editor = new BetterFolderBrowser();

                editor.editorService = editorService;
                editor.Multiselect = false;

                if (editor.ShowDialog() == DialogResult.OK)
                    value = editor.SelectedPath;
            }

            return value;
        }
    }

    /// <summary>
    /// Provides a custom <see cref="BetterFolderBrowser"/> UI Editor
    /// for browsing through folders via the Properties window. 
    /// This allows for the selection of a single folder.
    /// It's designed as a replacement for <see cref="FolderBrowserDialog"/>'s
    /// <see cref="UITypeEditor"/>.
    /// 
    /// <para>
    /// Example:
    /// <code>[Editor(typeof(BetterFolderBrowserPathsEditor), typeof(UITypeEditor))]</code>
    /// </para>
    /// </summary>
    [DebuggerStepThrough]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class SelectedPathsEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context,
                                IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService editorService =
               provider.GetService(typeof(IWindowsFormsEditorService)) as
               IWindowsFormsEditorService;

            if (editorService != null)
            {
                BetterFolderBrowser editor = new BetterFolderBrowser();

                editor.editorService = editorService;
                editor.Multiselect = true;

                if (editor.ShowDialog() == DialogResult.OK)
                    value = editor.SelectedPaths;
            }

            return value;
        }
    }
}

namespace WK.Libraries.BetterFolderBrowserNS.Helpers
{
    /// <summary>
    /// This class is from the Front-End for Dosbox and is 
    /// used to present a 'vista' dialog box to select folders.
    /// http://code.google.com/p/fed/
    ///
    /// For example:
    /// ----------------------------------------------
    /// var r = new Reflector("System.Windows.Forms");
    /// </summary>
    public class Reflector
    {
        #region Fields

        private string m_ns;
        private Assembly m_asmb;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <see cref="Reflector"/> class object.
        /// </summary>
        /// <param name="ns">The namespace containing types to be used.</param>
        public Reflector(string ns) : this(ns, ns) { }

        /// <summary>
        /// Creates a new <see cref="Reflector"/> class object.
        /// </summary>
        /// <param name="an__1">
        /// A specific assembly name (used if the assembly name does not tie exactly with the namespace)
        /// </param>
        /// <param name="ns">The namespace containing types to be used.</param>
        public Reflector(string an__1, string ns)
        {
            m_ns = ns;
            m_asmb = null;

            foreach (AssemblyName aN__2 in Assembly.GetExecutingAssembly().GetReferencedAssemblies())
            {
                if (aN__2.FullName.StartsWith(an__1))
                {
                    m_asmb = Assembly.Load(aN__2);
                    break;
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Return a Type instance for a type 'typeName'.
        /// </summary>
        /// <param name="typeName">The name of the type.</param>
        /// <returns>A type instance.</returns>
        public Type GetTypo(string typeName)
        {
            Type type = null;
            string[] names = typeName.Split('.');

            if (names.Length > 0)
            {
                type = m_asmb.GetType((m_ns + Convert.ToString(".")) + names[0]);
            }

            for (int i = 1; i < names.Length; i++)
            {
                type = type.GetNestedType(names[i], BindingFlags.NonPublic);
            }

            return type;
        }

        /// <summary>
        /// Create a new object of a named type passing along any params.
        /// </summary>
        /// <param name="name">The name of the type to create.</param>
        /// <param name="parameters">An array of passed parameters.</param>
        /// <returns>An instantiated type.</returns>
        public object New(string name, params object[] parameters)
        {
            Type type = GetTypo(name);
            ConstructorInfo[] ctorInfos = type.GetConstructors();

            foreach (ConstructorInfo ci in ctorInfos)
            {
                try
                {
                    return ci.Invoke(parameters);
                }
                catch { }
            }

            return null;
        }

        /// <summary>
        /// Calls method 'func' on object 'obj' passing parameters 'parameters'.
        /// </summary>
        /// <param name="obj">The object on which to excute function 'func'.</param>
        /// <param name="func">The function to execute.</param>
        /// <param name="parameters">The parameters to pass to function 'func'.</param>
        /// <returns>The result of the function invocation.</returns>
        public object Call(object obj, string func, params object[] parameters)
        {
            return Call2(obj, func, parameters);
        }

        /// <summary>
        /// Calls method 'func' on object 'obj' passing parameters 'parameters'.
        /// </summary>
        /// <param name="obj">The object on which to excute function 'func'.</param>
        /// <param name="func">The function to execute.</param>
        /// <param name="parameters">The parameters to pass to function 'func'.</param>
        /// <returns>The result of the function invocation.</returns>
        public object Call2(Object obj, string func, object[] parameters)
        {
            return CallAs2(obj.GetType(), obj, func, parameters);
        }

        /// <summary>
        /// Calls method 'func' on object 'obj' which is of type 'type' passing parameters 'parameters'.
        /// </summary>
        /// <param name="type">The type of 'obj'.</param>
        /// <param name="obj">The object on which to excute function 'func'.</param>
        /// <param name="func">The function to execute.</param>
        /// <param name="parameters">The parameters to pass to function 'func'.</param>
        /// <returns>The result of the function invocation.</returns>
        public object CallAs(Type type, object obj, string func, params object[] parameters)
        {
            return CallAs2(type, obj, func, parameters);
        }

        /// <summary>
        /// Calls method 'func' on object 'obj' which is of type 'type' passing parameters 'parameters'.
        /// </summary>
        /// <param name="type">The type of 'obj'.</param>
        /// <param name="obj">The object on which to excute function 'func'.</param>
        /// <param name="func">The function to execute.</param>
        /// <param name="parameters">The parameters to pass to function 'func'.</param>
        /// <returns>The result of the function invocation.</returns>
        public object CallAs2(Type type, object obj, string func, object[] parameters)
        {
            MethodInfo methInfo = type.GetMethod(func, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            return methInfo.Invoke(obj, parameters);
        }

        /// <summary>
        /// Returns the value of property 'prop' of object 'obj'.
        /// </summary>
        /// <param name="obj">The object containing 'prop'.</param>
        /// <param name="prop">The property name.</param>
        /// <returns>The property value.</returns>
        public object Get(Object obj, string prop)
        {
            return GetAs(obj.GetType(), obj, prop);
        }

        /// <summary>
        /// Returns the value of property 'prop' of object 'obj' which has type 'type'.
        /// </summary>
        /// <param name="type">The type of 'obj'.</param>
        /// <param name="obj">The object containing 'prop'.</param>
        /// <param name="prop">The property name.</param>
        /// <returns>The property value.</returns>
        public object GetAs(Type type, object obj, string prop)
        {
            PropertyInfo propInfo = type.GetProperty(prop, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            return propInfo.GetValue(obj, null);
        }

        /// <summary>
        /// Returns an enum value.
        /// </summary>
        /// <param name="typeName">The name of enum type.</param>
        /// <param name="name">The name of the value.</param>
        /// <returns>The enum value.</returns>
        public object GetEnum(string typeName, string name)
        {
            Type type = GetTypo(typeName);
            FieldInfo fieldInfo = type.GetField(name);

            return fieldInfo.GetValue(null);
        }

        #endregion
    }

    /// <summary>
    /// Wraps System.Windows.Forms.OpenFileDialog to make it present
    /// a vista-style dialog.
    /// </summary>
    public class BetterFolderBrowserDialog
    {
        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public BetterFolderBrowserDialog()
        {
            ofd = new OpenFileDialog();

            ofd.Filter = "Folders|" + "\n";
            ofd.AddExtension = false;
            ofd.CheckFileExists = false;
            ofd.DereferenceLinks = true;
            ofd.Multiselect = false;
        }

        #endregion

        #region Fields

        private bool allow_multiselect;
        private OpenFileDialog ofd = null;

        #endregion

        #region Properties

        /// <summary>
        /// Gets/Sets a value indicating whether to allow multi-selection of folders.
        /// </summary>
        /// <remarks></remarks>
        public bool AllowMultiselect
        {
            get { return allow_multiselect; }
            set { ofd.Multiselect = value; }
        }

        /// <summary>
        /// Gets the list of selected folders as filenames.
        /// </summary>
        public string[] FileNames
        {
            get { return ofd.FileNames; }
        }

        /// <summary>
        /// Gets/Sets the initial folder to be selected. A null value selects the current directory.
        /// </summary>
        public string InitialDirectory
        {
            get { return ofd.InitialDirectory; }
            set
            {
                ofd.InitialDirectory = (value == null || value.Length == 0) ? Environment.CurrentDirectory : value;
            }
        }

        /// <summary>
        /// Gets/Sets the title to show in the dialog.
        /// </summary>
        public string Title
        {
            get { return ofd.Title; }
            set { ofd.Title = (value == null) ? "Select a folder" : value; }
        }

        /// <summary>
        /// Gets the selected folder.
        /// </summary>
        public string FileName
        {
            get { return ofd.FileName; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Shows the dialog.
        /// </summary>
        /// <returns>True if the user presses OK else false.</returns>
        public bool ShowDialog()
        {
            return ShowDialog(IntPtr.Zero);
        }

        /// <summary>
        /// Shows the dialog.
        /// </summary>
        /// <param name="hWndOwner">Handle of the control to be parent.</param>
        /// <returns>True if the user presses OK else false.</returns>
        public bool ShowDialog(IntPtr hWndOwner)
        {
            bool flag = false;

            if (Environment.OSVersion.Version.Major >= 6)
            {
                var r = new Reflector("System.Windows.Forms");

                uint num = 0;
                Type typeIFileDialog = r.GetTypo("FileDialogNative.IFileDialog");
                object dialog = r.Call(ofd, "CreateVistaDialog");
                r.Call(ofd, "OnBeforeVistaDialog", dialog);

                uint options = Convert.ToUInt32(r.CallAs(typeof(System.Windows.Forms.FileDialog), ofd, "GetOptions"));
                options |= Convert.ToUInt32(r.GetEnum("FileDialogNative.FOS", "FOS_PICKFOLDERS"));
                r.CallAs(typeIFileDialog, dialog, "SetOptions", options);

                object pfde = r.New("FileDialog.VistaDialogEvents", ofd);
                object[] parameters = new object[] { pfde, num };
                r.CallAs2(typeIFileDialog, dialog, "Advise", parameters);

                num = Convert.ToUInt32(parameters[1]);

                try
                {
                    int num2 = Convert.ToInt32(r.CallAs(typeIFileDialog, dialog, "Show", hWndOwner));
                    flag = 0 == num2;
                }
                finally
                {
                    r.CallAs(typeIFileDialog, dialog, "Unadvise", num);
                    GC.KeepAlive(pfde);
                }
            }
            else
            {
                var fbd = new FolderBrowserDialog();

                fbd.Description = this.Title;
                fbd.SelectedPath = this.InitialDirectory;
                fbd.ShowNewFolderButton = false;

                if (fbd.ShowDialog(new WindowWrapper(hWndOwner)) != DialogResult.OK)
                {
                    return false;
                }

                ofd.FileName = fbd.SelectedPath;
                flag = true;
            }

            return flag;
        }

        #endregion
    }

    /// <summary>
    /// Creates IWin32Window around an IntPtr.
    /// </summary>
    public class WindowWrapper : IWin32Window
    {
        #region Fields

        private IntPtr _hwnd;

        #endregion

        #region Properties

        /// <summary>
        /// Original pointer.
        /// </summary>
        public IntPtr Handle
        {
            get { return _hwnd; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Provides a wrapper for <see cref="IWin32Window"/>.
        /// </summary>
        /// <param name="handle">Handle to wrap.</param>
        public WindowWrapper(IntPtr handle)
        {
            _hwnd = handle;
        }

        #endregion
    }
}