import 'package:flutter/material.dart';

class EditorNewProjectDialog extends StatefulWidget {
  const EditorNewProjectDialog({super.key});

  @override
  State<EditorNewProjectDialog> createState() => _EditorNewProjectDialogState();
}

class _EditorNewProjectDialogState extends State<EditorNewProjectDialog> {
  late final GlobalKey formKey;

  late String name;
  late String bundleId;

  @override
  void initState() {
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
            width: 400.0,
            height: 350.0,
            child: Column(
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
                  validator: (value) {
                    if (value == null || value.isEmpty) return 'Proivde a name';
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
                  validator: (value) {
                    if (value == null || value.isEmpty) return 'Proivde a Bundle ID';
                    return null;
                  },
                  onSaved: (value) => bundleId = value!,
                ),
                SizedBox(height: 24.0),
                Expanded(child: Container()),
                FilledButton(
                  onPressed: () {
                    final form = formKey.currentState as FormState;
                    if (form.validate()) {
                      form.save();
                      Navigator.pop(context, [name, bundleId]);
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
