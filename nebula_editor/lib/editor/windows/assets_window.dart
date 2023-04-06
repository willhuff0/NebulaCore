import 'dart:async';

import 'package:flutter/material.dart';
import 'package:nebula_editor/editor/editor_context.dart';
import 'package:nebula_editor/nebula.dart';
import 'package:resizable_widget/resizable_widget.dart';

import '../editor_page.dart';

class EditorWindowAssets extends StatefulWidget {
  const EditorWindowAssets({super.key});

  @override
  State<EditorWindowAssets> createState() => _EditorWindowAssetsState();
}

class _EditorWindowAssetsState extends State<EditorWindowAssets> {
  late final StreamSubscription _onProjectLoadedSubscription;

  NbProject? project;
  var showImportedAssets = true;

  NbAssetGroup? selectedGroup;
  List<MapEntry<String, NbAsset>>? entries;

  void selectGroup(NbAssetGroup? group) {
    setState(() {
      selectedGroup = group;
      entries = selectedGroup?.entries.toList();
    });
  }

  @override
  void initState() {
    WidgetsBinding.instance.addPostFrameCallback((timeStamp) {
      _onProjectLoadedSubscription = EditorContext.onProjectLoaded.listen((project) {
        setState(() => project = project);
      });
    });
    super.initState();
  }

  @override
  void dispose() {
    _onProjectLoadedSubscription.cancel();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    if (project == null) {
      return Center(child: Text('No project loaded'));
    }
    final assets = showImportedAssets ? project!.allAssets : project!.assets;
    return Row(
      children: [
        Container(
          width: 28.0,
          color: Theme.of(context).colorScheme.surfaceTint.withOpacity(0.1),
          padding: EdgeInsets.symmetric(vertical: 4.0),
          child: Column(
            children: [
              MenuAnchor(
                style: MenuStyle(alignment: Alignment.topRight, visualDensity: VisualDensity(horizontal: VisualDensity.minimumDensity, vertical: VisualDensity.minimumDensity)),
                alignmentOffset: Offset(4.0, 0.0),
                menuChildren: [
                  MenuItemButton(
                    leadingIcon: Icon(Icons.create_rounded),
                    child: Text('Create asset'),
                    onPressed: () {},
                  ),
                  MenuItemButton(
                    leadingIcon: Icon(Icons.upload_file_rounded),
                    child: Text('Import asset'),
                    onPressed: () {},
                  ),
                ],
                builder: (context, controller, child) => IconButton(
                  onPressed: () => controller.open(),
                  padding: EdgeInsets.zero,
                  iconSize: 20.0,
                  tooltip: 'New asset',
                  icon: Icon(Icons.add_circle_rounded),
                ),
              ),
              MenuAnchor(
                style: MenuStyle(alignment: Alignment.topRight, visualDensity: VisualDensity(horizontal: VisualDensity.minimumDensity, vertical: VisualDensity.minimumDensity)),
                menuChildren: [
                  CreateAssetCollectionDialog(
                    onCreateAssetCollection: () => setState(() {}),
                  ),
                ],
                builder: (context, controller, child) => IconButton(
                  icon: Icon(Icons.create_new_folder_rounded),
                  iconSize: 20.0,
                  tooltip: 'Create asset collection',
                  padding: EdgeInsets.zero,
                  onPressed: () => controller.open(),
                ),
              ),
            ],
          ),
        ),
        Expanded(
          child: ResizableWidget(
            children: [
              Padding(
                padding: const EdgeInsets.all(14.0),
                child: SizedBox.expand(
                  child: Wrap(
                    spacing: 8.0,
                    runSpacing: 8.0,
                    children: [
                      ...assets.entries
                          .map((collection) => FilledButton.tonal(
                                style: ButtonStyle(
                                  shape: MaterialStatePropertyAll(RoundedRectangleBorder(borderRadius: BorderRadius.circular(14.0))),
                                  padding: MaterialStatePropertyAll(EdgeInsets.all(20.0)),
                                ),
                                child: Row(
                                  mainAxisSize: MainAxisSize.min,
                                  children: [
                                    Icon(Icons.folder),
                                    SizedBox(width: 8.0),
                                    Text(collection.key),
                                  ],
                                ),
                                onPressed: () {},
                              ))
                          .toList(),
                      ...NbAsset.deserializers.entries
                          .where((element) => !assets.containsKey(element.key))
                          .map((deserializer) => FilledButton.tonal(
                                style: ButtonStyle(
                                  shape: MaterialStatePropertyAll(RoundedRectangleBorder(borderRadius: BorderRadius.circular(14.0))),
                                  padding: MaterialStatePropertyAll(EdgeInsets.all(20.0)),
                                ),
                                onPressed: null,
                                child: Row(
                                  mainAxisSize: MainAxisSize.min,
                                  children: [
                                    Icon(Icons.folder),
                                    SizedBox(width: 8.0),
                                    Text(deserializer.key),
                                  ],
                                ),
                              ))
                          .toList()
                    ],
                  ),
                ),
              ),
              if (selectedGroup != null)
                Padding(
                  padding: EdgeInsets.all(8.0),
                  child: ListView.builder(
                    itemBuilder: (context, index) {
                      final asset = entries![index];
                      return ListTile(
                        dense: true,
                        visualDensity: VisualDensity(horizontal: VisualDensity.maximumDensity, vertical: VisualDensity.maximumDensity),
                        title: Text(asset.value.name),
                        subtitle: Text(asset.key),
                      );
                    },
                    itemCount: selectedGroup!.length,
                  ),
                ),
            ],
          ),
        ),
      ],
    );
  }
}

class CreateAssetCollectionDialog extends StatefulWidget {
  final VoidCallback onCreateAssetCollection;

  const CreateAssetCollectionDialog({super.key, required this.onCreateAssetCollection});

  @override
  State<CreateAssetCollectionDialog> createState() => _CreateAssetCollectionDialogState();
}

class _CreateAssetCollectionDialogState extends State<CreateAssetCollectionDialog> {
  var name = '';

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      width: 300.0,
      height: 110.0,
      child: Padding(
        padding: const EdgeInsets.all(14.0),
        child: Column(
          children: [
            TextField(
              decoration: InputDecoration(
                isDense: true,
                border: OutlineInputBorder(borderSide: BorderSide.none),
                filled: true,
                labelText: 'Name',
                hintText: 'assets',
              ),
              onChanged: (value) => name = value,
            ),
            SizedBox(height: 14.0),
            Row(
              children: [
                Text('New asset collection'),
                Expanded(child: Container()),
                FilledButton.tonal(
                  onPressed: () {
                    if (name.isEmpty) return;
                    //EditorContext.activeProject!.assetBundle.createAssetCollection(name);
                    widget.onCreateAssetCollection();
                    Navigator.pop(context);
                  },
                  child: Text('Create'),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}
