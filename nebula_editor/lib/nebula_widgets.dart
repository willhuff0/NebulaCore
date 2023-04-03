import 'package:flutter/material.dart';
import 'package:nebula_editor/nebula.dart';

const nodePropertiesPollDelay = 0.5;

class NodePropertyPanel extends StatefulWidget {
  final NebulaNode node;

  const NodePropertyPanel({super.key, required this.node});

  @override
  State<NodePropertyPanel> createState() => _NodePropertyPanelState();
}

class _NodePropertyPanelState extends State<NodePropertyPanel> {
  @override
  void initState() {
    poll();
    super.initState();
  }

  void poll() async {
    while (mounted) {
      await widget.node.syncProperties();
      await Future.delayed(Duration(milliseconds: (nodePropertiesPollDelay * 1000).toInt()));
    }
  }

  @override
  Widget build(BuildContext context) {
    return const Placeholder();
  }
}

class NodePropertyTile extends StatelessWidget {
  const NodePropertyTile({super.key});

  @override
  Widget build(BuildContext context) {
    return const Placeholder();
  }
}
