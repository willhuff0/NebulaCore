import 'dart:async';

import 'package:flutter/material.dart';
import 'package:fuzzywuzzy/fuzzywuzzy.dart';
import 'package:nebula_editor/editor/editor_context.dart';
import 'package:nebula_editor/nebula.dart';

const nodePropertiesPollDelay = 0.5;

class NodePropertyPanel extends StatefulWidget {
  final NbNode node;

  const NodePropertyPanel({super.key, required this.node});

  @override
  State<NodePropertyPanel> createState() => _NodePropertyPanelState();
}

class _NodePropertyPanelState extends State<NodePropertyPanel> {
  late final Map<String, bool> panelsExpanded;

  @override
  void initState() {
    panelsExpanded = <String, bool>{};
    poll();
    super.initState();
  }

  void poll() async {
    while (mounted) {
      await widget.node.syncProperties();
      setState(() {});
      await Future.delayed(Duration(milliseconds: (nodePropertiesPollDelay * 1000).toInt()));
    }
  }

  @override
  Widget build(BuildContext context) {
    var i = 0;
    var expansionCallbacks = <int, void Function(bool expanded)>{};
    return ExpansionPanelList(
      children: widget.node.properties.entries.map((behavior) {
        var panel = ExpansionPanel(
          isExpanded: panelsExpanded[behavior.key] ?? false,
          headerBuilder: (context, expanded) {
            return Text(behavior.key);
          },
          body: Column(
            children: behavior.value.entries.map((property) {
              PropertyTileTypeDescriptor descriptor;
              switch (property.value.runtimeType) {
                case bool:
                  descriptor = PropertyTileTypeDescriptor(PropertyTileType.toggle, bool, null);
                  break;
                case NbAsset:
                  descriptor = PropertyTileTypeDescriptor(PropertyTileType.asset, property.value.runtimeType, property.value.groupKey);
                  break;
                default:
                  descriptor = PropertyTileTypeDescriptor(PropertyTileType.text, property.value.runtimeType, null);
                  break;
              }
              return PropertyTile(
                name: property.key,
                descriptor: descriptor,
                value: property.value,
                onChanged: (value) {
                  widget.node.setProperty(behavior.key, property.key, value);
                },
              );
            }).toList(),
          ),
        );
        expansionCallbacks[i] = (expanded) => setState(() => panelsExpanded[behavior.key] = expanded);
        i++;
        return panel;
      }).toList(),
      expansionCallback: (index, expanded) => expansionCallbacks[index]!(expanded),
    );
  }
}

enum PropertyTileType { text, toggle, asset, node }

class PropertyTileTypeDescriptor {
  final PropertyTileType type;
  final Type rawType;
  final String? subtype;

  PropertyTileTypeDescriptor(this.type, this.rawType, this.subtype);
}

class PropertyTile extends StatefulWidget {
  final String name;
  final PropertyTileTypeDescriptor descriptor;
  final dynamic value;
  final void Function(dynamic value) onChanged;

  const PropertyTile({super.key, required this.name, required this.descriptor, required this.value, required this.onChanged});

  @override
  State<PropertyTile> createState() => _PropertyTileState();
}

class _PropertyTileState extends State<PropertyTile> {
  late final TextEditingController _controller;

  @override
  void initState() {
    if (widget.descriptor.type == PropertyTileType.text) _controller = TextEditingController(text: widget.value.toString());
    super.initState();
  }

  Widget getEditField() {
    switch (widget.descriptor.type) {
      case PropertyTileType.toggle:
        return Checkbox(value: widget.value as bool, onChanged: widget.onChanged);
      case PropertyTileType.text:
        return TextField(
          controller: _controller,
          decoration: InputDecoration(
            border: OutlineInputBorder(borderSide: BorderSide.none),
          ),
          onEditingComplete: () {
            widget.onChanged(_controller.text);
          },
        );
      case PropertyTileType.asset:
        return MenuAnchor(
          menuChildren: [
            AssetSelectionDialog(
              group: widget.descriptor.subtype!,
              onTap: (asset) {
                widget.onChanged(asset);
              },
            ),
          ],
          builder: (context, controller, child) => FilledButton.tonal(
            onPressed: () {
              controller.open();
            },
            child: child,
          ),
        );
      case PropertyTileType.node:
        return Placeholder();
    }
  }

  @override
  Widget build(BuildContext context) {
    if (widget.descriptor.type == PropertyTileType.text) _controller.text = widget.value.toString();
    return Row(
      children: [
        Text(widget.name, style: Theme.of(context).textTheme.labelMedium),
        Expanded(child: getEditField()),
      ],
    );
  }
}

class AssetSelectionDialog extends StatefulWidget {
  final String group;
  final void Function(NbAsset? result) onTap;

  const AssetSelectionDialog({super.key, required this.group, required this.onTap});

  @override
  State<AssetSelectionDialog> createState() => _AssetSelectionDialogState();
}

class _AssetSelectionDialogState extends State<AssetSelectionDialog> {
  late final TextEditingController _searchFieldController;
  Timer? _debounce;
  List<MapEntry<String, NbAsset>>? search;

  _onSearchChanged(String query) {
    _debounce?.cancel();
    _debounce = Timer(
      const Duration(milliseconds: 500),
      () => setState(() => search = query.isEmpty
          ? null
          : extractAllSorted<MapEntry<String, NbAsset>>(
              query: query,
              choices: EditorContext.activeProject!.allAssets[widget.group]!.entries.toList(),
              getter: (obj) => obj.value.name,
              cutoff: 20,
            ).map((e) => e.choice).toList()),
    );
  }

  @override
  void initState() {
    _searchFieldController = TextEditingController();
    _onSearchChanged('');
    super.initState();
  }

  @override
  void dispose() {
    _searchFieldController.dispose();
    _debounce?.cancel();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final entries = search ?? EditorContext.activeProject!.allAssets[widget.group]!.entries;
    return SizedBox(
      width: 600.0,
      height: 500.0,
      child: Column(
        children: [
          TextField(
            controller: _searchFieldController,
            onChanged: _onSearchChanged,
            decoration: InputDecoration(
              border: OutlineInputBorder(borderSide: BorderSide.none),
              filled: true,
              labelText: 'Search',
              suffix: IconButton(
                icon: Icon(Icons.cancel_rounded),
                onPressed: () {
                  _searchFieldController.clear();
                },
              ),
            ),
          ),
          Expanded(
            child: ListView.builder(
              padding: EdgeInsets.all(14.0),
              itemCount: search == null ? null : entries.length,
              itemBuilder: (context, index) {
                if (!entries.iterator.moveNext()) return null;
                final entry = entries.iterator.current;
                return ListTile(
                  dense: true,
                  visualDensity: VisualDensity(horizontal: VisualDensity.maximumDensity, vertical: VisualDensity.maximumDensity),
                  title: Text(entry.value.name),
                  subtitle: Text(entry.key),
                  onTap: () {
                    widget.onTap(entry.value);
                    Navigator.pop(context, entry.value);
                  },
                );
              },
            ),
          ),
        ],
      ),
    );
  }
}
