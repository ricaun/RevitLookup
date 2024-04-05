54a4d529c278ecc4a3d60450f7c74586259f25dc | 12.12.2023

# Common

- Application.Current changed to Application.MainMenu static property

# Wpf.Ui.Appearance

- Updated uris

# Wpf.Ui.Appearance.ApplicationAccentColorManager

- GetColorizationColor disable for Windows 10

# Wpf.Ui.Controls.TitleBarControl

- Application.Current.Shutdown() removed

# Wpf.Ui.Controls.TreeView

- New control with ItemsSourceChanged event

# Wpf.Ui.Controls.ContentDialog

- ShowAsync add catch (OperationCanceledException)

# Wpf.Ui.Controls.NavigationView

- Changed s_titleBarPaneCompactMargin
- Changed IsPaneOpen default value to false
- LeftCompactNavigationViewItemTemplate MinWidth removed
- LeftCompactNavigationViewItemTemplate IconContentPresenter font size set 18
- LeftNavigationViewTemplate Trigger IsPaneOpen for PaneGrid width set 40
- LeftNavigationViewTemplate PART_BackButton margin set 0,3,0,5
- 
# Wpf.Ui.Controls.NavigationViewContentPresenter

- Disabled CanContentScrollProperty override

# Wpf.Ui.Controls.DataGrid

- ItemsSourceChanged event
- Removed combobox events
- DefaultDataGridFontSize set 12
- DefaultDataGridCellStyle MinHeight set 26
- DefaultDataGridCellStyle Border set VerticalAlignment="Center" Margin="4,0,4,0"
- DefaultDataGridColumnHeaderStyle MinHeight set 26
- DefaultDataGridColumnHeaderStyle columnHeaderBorder set Padding="7,0,7,0"
- DefaultDataGridColumnHeaderStyle columnHeaderBorder ContentPresenter set TextBlock.FontWeight="Bold"
- DefaultDataGridStyle PART_VerticalScrollBar set Grid.RowSpan
- VerticalScrollBarVisibility binding

# Wpf.Ui.Controls.ContentDialog

- Removed ResizeToContentSize
- Changed DefaultDataGridFontSize
- Changed DefaultDataGridCellStyle MinHeight
- Changed DefaultDataGridCellStyle ContentPresenter VerticalAlignment, Margin
- Changed DefaultDataGridColumnHeaderStyle MinHeight
- Changed DefaultDataGridColumnHeaderStyle ContentPresenter VerticalAlignment, Margin
- Changed DefaultDataGridStyle VerticalScrollBar Grid.RowSpan

# Wpf.Ui.Controls.TreeViewItem

- TreeViewItemFontSize set 12
- Add TreeViewItem.xaml.cs
- Add RequestBringIntoView
- Set VirtualizationMode="Recycling"

# Wpf.Ui.Controls.TreeView

- Add style for Wpf.Ui.Controls.TreeView

# Wpf.Ui.Controls.ToolTip

- MaxWidth set 600
- Set TextElement.FontWeight Normal

# Wpf.Ui.Controls.Contextmenu

- Set TextElement.FontWeight Normal
- Changed BorderBrush

# Wpf.Ui.Controls.TextBlock

- FontSize set 14
- Added Foreground for page support

# Wpf.Ui.Controls.TextBox

- Changed toplevel Thickness
- VerticalAlignment set Top

# Wpf.Ui.Controls.MenuItem

- WpfUiMenuItemSubmenuItemTemplateKey removed border

# Wpf.Ui.Controls.Snackbar

- Add message margin
- Set new TextSize

# Wpf.Ui.Controls.IconElement.FontIcon

- Removed (FontSize.Equals(SystemFonts.MessageFontSize))
- Removed (VisualParent is not null)

# Wpf.Ui.Resources.Typography

- TitleTextBlockStyle FontSize set 18
- TitleTextBlockStyle LineHeight set 18

# Wpf.Ui.Controls.WindowBackdrop

- Increased WindowBackdropType.Acrylic supported OS