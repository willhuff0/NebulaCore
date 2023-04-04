import 'package:flutter/material.dart';
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
      children: widget.node.properties.entries.map((e) {
        var panel = ExpansionPanel(
          isExpanded: panelsExpanded[e.key] ?? false,
          headerBuilder: (context, expanded) {
            return Text(e.key);
          },
          body: Column(
            children: e.value.entries.map((e) {
              return NodePropertyTile(
                name: e.key,
                type: e.value.runtimeType,
                value: e.value,
                onChanged: (value) {},
              );
            }).toList(),
          ),
        );
        expansionCallbacks[i] = (expanded) => setState(() => panelsExpanded[e.key] = expanded);
        i++;
        return panel;
      }).toList(),
      expansionCallback: (index, expanded) => expansionCallbacks[index]!(expanded),
    );
  }
}

class NodePropertyTile extends StatefulWidget {
  final String name;
  final Type type;
  final dynamic value;
  final void Function(dynamic value) onChanged;

  const NodePropertyTile({super.key, required this.name, required this.type, required this.value, required this.onChanged});

  @override
  State<NodePropertyTile> createState() => _NodePropertyTileState();
}

class _NodePropertyTileState extends State<NodePropertyTile> {
  late final TextEditingController _controller;

  @override
  void initState() {
    if (widget.type != bool) _controller = TextEditingController(text: widget.value.toString());
    super.initState();
  }

  Widget getEditField() {
    switch (widget.type) {
      case bool:
        return Checkbox(value: widget.value as bool, onChanged: widget.onChanged);
      default:
        return TextField(
          controller: _controller,
          decoration: InputDecoration(
            border: OutlineInputBorder(borderSide: BorderSide.none),
          ),
        );
    }
  }

  @override
  Widget build(BuildContext context) {
    _controller.text = widget.value.toString();
    return Row(
      children: [
        Text(widget.name, style: Theme.of(context).textTheme.labelMedium),
        Expanded(child: getEditField()),
      ],
    );
  }
}
