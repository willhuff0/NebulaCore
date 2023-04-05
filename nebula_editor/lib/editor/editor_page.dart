import 'package:flutter/material.dart';

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

  @override
  Widget build(BuildContext context) {
    return Scaffold(
        appBar: AppBar(
          toolbarHeight: 24.0,
          elevation: 8.0,
          centerTitle: false,
          title: Row(
            children: [
              Text('Nebula Editor${ready && editorContext.nebula.activeProject != null ? ' - ${editorContext.nebula.activeProject!.name}' : ''}', style: Theme.of(context).textTheme.titleSmall),
              SizedBox(width: 12.0),
              MenuAnchor( // Project Anchor
                style: MenuStyle(visualDensity: VisualDensity(horizontal: VisualDensity.minimumDensity, vertical: VisualDensity.minimumDensity)),
                menuChildren: [
                  MenuItemButton(
                    leadingIcon: Icon(Icons.file_copy_rounded, size: 20.0),
                    child: Text('New'),
                    onPressed: () async {
                      final result = await showDialog<List?>(context: context, builder: (context) => EditorNewProjectDialog());
                      if (result == null) return;
                      //await editorContext.createProject(result[0], result[1]);
                      setState(() {});
                    },
                  ),
                  MenuItemButton(
                    leadingIcon: Icon(Icons.file_open_rounded, size: 20.0),
                    child: Text('Open'),
                    onPressed: () {
                      //EditorContext.openProject();
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
                    onPressed: () {},
                  ),
                ],
                builder: (context, controller, child) => TextButton.icon(
                  style: ButtonStyle(
                    shape: MaterialStatePropertyAll(RoundedRectangleBorder(borderRadius: BorderRadius.circular(8.0)))
                  ),
                  icon: Icon(Icons.folder_rounded, size: 18.0),
                  label: Text('Project'),
                  onPressed: ready ? () {
                    controller.open();
                  } : null,
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