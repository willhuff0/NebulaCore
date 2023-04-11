import 'package:file_picker/file_picker.dart';
import 'package:flutter/material.dart';
import 'package:nebula_editor/nebula/project.dart';
import 'package:path/path.dart' as p;

import 'package:resizable_widget/resizable_widget.dart';

import 'dialogs/new_project.dart';
import 'editor_context.dart';
import 'editor_widgets.dart';
import 'editor_windows.dart';

class Editor extends StatefulWidget {
  const Editor({super.key});

  @override
  State<Editor> createState() => _EditorState();
}

class _EditorState extends State<Editor> {
  late final List<(double, List<(double, EditorZone)>)> grid;

  @override
  void initState() {
    grid = [
      (0.7, [
        (0.2, EditorZone(tabs: [EditorWindow(key: GlobalKey(), config: EditorWindowConfig('Layout', EditorWindowLayout(key: GlobalKey())))])),
        (0.6, EditorZone(tabs: [EditorWindow(key: GlobalKey(), config: EditorWindowConfig('Viewport', EditorWindowViewport(key: GlobalKey())))])),
        (0.2, EditorZone(tabs: [EditorWindow(key: GlobalKey(), config: EditorWindowConfig('Properties', EditorWindowProperties(key: GlobalKey())))])),
      ],),
      (0.3, [
        (0.525, EditorZone(tabs: [EditorWindow(key: GlobalKey(), config: EditorWindowConfig('Assets', EditorWindowAssets(key: GlobalKey())))])),
        (0.475, EditorZone(tabs: [EditorWindow(key: GlobalKey(), config: EditorWindowConfig('Debugger', EditorWindowDebugger(key: GlobalKey())))])),
      ],),
    ];
    super.initState();
  }

  Future<bool> showCloseOpenProjectFirstDialog() async{
    final result = await showDialog<int?>(context: context, builder: (context) => AlertDialog(
                          title: Text('Close ${EditorContext.activeProject!.name} before opening another project?'),
                          actions: [
                            TextButton(onPressed: () => Navigator.pop(context, 0), child: Text('Cancel')),
                            FilledButton.tonal(onPressed: () => Navigator.pop(context, 1), child: Text('Discard Changes')),
                            FilledButton(onPressed: () => Navigator.pop(context, 2), child: Text('Save Changes')),
                          ],
                        )) ?? 0;
                        
                        switch(result) {
                          case 1:
                          return true;
                          case 2:
  await EditorContext.activeProject!.save();
  
                          return true;
                          default:
                          return false;
                        }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
        appBar: AppBar(
          toolbarHeight: 24.0,
          elevation: 8.0,
          centerTitle: false,
          title: Row(
            children: [
              Text('Nebula Editor${EditorContext.activeProject != null ? ' - ${EditorContext.activeProject!.name}' : ''}', style: Theme.of(context).textTheme.titleSmall),
              SizedBox(width: 12.0),
              MenuAnchor( // Project Anchor
                style: MenuStyle(visualDensity: VisualDensity(horizontal: VisualDensity.minimumDensity, vertical: VisualDensity.minimumDensity)),
                menuChildren: [
                  MenuItemButton(
                    leadingIcon: Icon(Icons.file_copy_rounded, size: 20.0),
                    child: Text('New'),
                    onPressed: () async {
                      final result = await showDialog<NbProject?>(context: context, builder: (context) => EditorNewProjectDialog());
                      if (result == null) return;
                      EditorContext.activeProject = result;
                      setState(() {});
                    },
                  ),
                  MenuItemButton(
                    leadingIcon: Icon(Icons.file_open_rounded, size: 20.0),
                    child: Text('Open'),
                    onPressed: () async {
                      if (EditorContext.activeProject != null) {
                        if(await showCloseOpenProjectFirstDialog() == false) return;
                      }

                      final path = await FilePicker.platform.pickFiles(
                          initialDirectory: EditorContext.activeProject != null ? p.dirname(EditorContext.activeProject!.path) : null,
                          dialogTitle: 'Select Nebula project file',
                          lockParentWindow: true,
                        );
                        if (!mounted || path == null || path.paths.isEmpty) return;

                      EditorContext.activeProject = await NbProject.loadProject(path.paths.first!);
                      setState(() {});
                    },
                  ),
                  Divider(),
                  MenuItemButton(
                    leadingIcon: Icon(Icons.save_rounded, size: 20.0),
                    child: Text('Save All'),
                    onPressed: () {
                      //EditorContext.nebula.activeProject?.saveProjectFile();
                    },
                  ),
                  Divider(),
                  MenuItemButton(
                    leadingIcon: Icon(Icons.logout_rounded, size: 20.0),
                    child: Text('Close'),
                    onPressed: () async{
                      await NbProject.unloadProject();
                    },
                  ),
                ],
                builder: (context, controller, child) => TextButton.icon(
                  style: ButtonStyle(
                    shape: MaterialStatePropertyAll(RoundedRectangleBorder(borderRadius: BorderRadius.circular(8.0)))
                  ),
                  icon: Icon(Icons.folder_rounded, size: 18.0),
                  label: Text('Project'),
                  onPressed: () {
                    controller.open();
                  },
                ),
              ),
              MenuAnchor( // Scene Anchor
                style: MenuStyle(visualDensity: VisualDensity(horizontal: VisualDensity.minimumDensity, vertical: VisualDensity.minimumDensity)),
                menuChildren: [
                  MenuItemButton(
                    leadingIcon: Icon(Icons.file_copy_rounded, size: 20.0),
                    child: Text('New'),
                    onPressed: () {},
                  ),
                  Divider(),
                  MenuItemButton(
                    leadingIcon: Icon(Icons.save_rounded, size: 20.0),
                    child: Text('Save Scene'),
                    onPressed: () {},
                  ),
                ],
                builder: (context, controller, child) => TextButton.icon(
                  style: ButtonStyle(
                    shape: MaterialStatePropertyAll(RoundedRectangleBorder(borderRadius: BorderRadius.circular(8.0)))
                  ),
                  icon: Icon(Icons.landscape_rounded, size: 20.0),
                  label: Text('Scene'),
                  onPressed: () {
                    controller.open();
                  },
                ),
              ),
            ],
          ),
        ),
        body: ResizableWidget(
          isHorizontalSeparator: true,
          percentages: grid.map((e) => e.$1).toList(),
          children: grid
              .map((column) => ResizableWidget(
                    isHorizontalSeparator: false,
                    percentages: column.$2.map((row) => row.$1).toList(),
                    children: column.$2.map((row) => row.$2).toList(),
                  ))
              .toList(),
        ),
    );
  }
}