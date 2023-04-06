import 'dart:io';

import 'package:file_picker/file_picker.dart';
import 'package:flutter/material.dart';
import 'package:nebula_editor/nebula.dart';
import 'package:path/path.dart' as p;

class EditorNewProjectDialog extends StatefulWidget {
  const EditorNewProjectDialog({super.key});

  @override
  State<EditorNewProjectDialog> createState() => _EditorNewProjectDialogState();
}

class _EditorNewProjectDialogState extends State<EditorNewProjectDialog> {
  late final TextEditingController _directoryController;

  late final GlobalKey formKey;

  late String name;
  late String bundleId;
  late String directory;

  var loading = false;

  @override
  void initState() {
    _directoryController = TextEditingController(text: Platform.environment['HOME'] ?? Platform.environment['USERPROFILE']);
    formKey = GlobalKey();
    super.initState();
  }

  @override
  Widget build(BuildContext context) {
    return Dialog(
      child: Form(
        key: formKey,
        child: Padding(
          padding: const EdgeInsets.symmetric(horizontal: 24.0, vertical: 36.0),
          child: SizedBox(
            width: 500.0,
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                Text('New Project', style: Theme.of(context).textTheme.titleMedium),
                SizedBox(height: 48.0),
                TextFormField(
                  decoration: InputDecoration(
                    border: OutlineInputBorder(borderSide: BorderSide.none),
                    filled: true,
                    labelText: 'Name',
                    hintText: 'MyGame',
                  ),
                  enabled: !loading,
                  validator: (value) {
                    if (value == null || value.isEmpty) return 'Provide a name';
                    return null;
                  },
                  onSaved: (value) => name = value!,
                ),
                SizedBox(height: 14.0),
                TextFormField(
                  decoration: InputDecoration(
                    border: OutlineInputBorder(borderSide: BorderSide.none),
                    filled: true,
                    labelText: 'Bundle ID',
                    hintText: 'com.myCompany.myGame',
                  ),
                  enabled: !loading,
                  validator: (value) {
                    if (value == null || value.isEmpty) return 'Provide a Bundle ID';
                    return null;
                  },
                  onSaved: (value) => bundleId = value!,
                ),
                SizedBox(height: 24.0),
                Row(
                  children: [
                    Expanded(
                      child: TextFormField(
                        controller: _directoryController,
                        minLines: 1,
                        maxLines: 3,
                        decoration: InputDecoration(
                          border: OutlineInputBorder(borderSide: BorderSide.none),
                          filled: true,
                          labelText: 'Path',
                        ),
                        enabled: !loading,
                        validator: (value) {
                          if (value == null || value.isEmpty) return 'Provide a path';
                          try {
                            final dir = Directory(value)..createSync(recursive: true);
                            if (dir.listSync().map((e) {
                              print(e);
                              return e;
                            }).isNotEmpty) return 'Directory must be empty';
                          } catch (e) {
                            print(e);
                            return 'An error occured while trying to create the directory';
                          }
                        },
                        onSaved: (value) => directory = value!,
                      ),
                    ),
                    SizedBox(width: 14.0),
                    FilledButton.tonal(
                      style: ButtonStyle(
                        padding: MaterialStatePropertyAll(EdgeInsets.zero),
                        minimumSize: MaterialStatePropertyAll(Size(65.0, 65.0)),
                        maximumSize: MaterialStatePropertyAll(Size(65.0, 65.0)),
                        shape: MaterialStatePropertyAll(RoundedRectangleBorder(borderRadius: BorderRadius.circular(8.0))),
                      ),
                      onPressed: () async {
                        final dir = await FilePicker.platform.getDirectoryPath(
                          initialDirectory: _directoryController.text,
                          dialogTitle: 'Create Nebula project file in an empty directory',
                          lockParentWindow: true,
                        );
                        if (!mounted || dir == null) return;
                        setState(() => _directoryController.text = dir);
                      },
                      child: Icon(Icons.folder_open_rounded),
                    ),
                  ],
                ),
                SizedBox(height: 48.0),
                FilledButton(
                  onPressed: loading
                      ? null
                      : () async {
                          try {
                            setState(() => loading = true);
                            final form = formKey.currentState as FormState;
                            if (form.validate()) {
                              form.save();
                              final project = await NbProject.createAndLoadProject(p.join(directory, '$name.neb'), name, bundleId);
                              if (!mounted) return;
                              Navigator.pop(context, project);
                            }
                          } finally {
                            setState(() => loading = false);
                          }
                        },
                  child: Text('Create and open'),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
