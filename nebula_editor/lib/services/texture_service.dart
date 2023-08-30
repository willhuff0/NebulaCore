// ignore_for_file: constant_identifier_names

import 'dart:io';

enum ASTCQuality {
  COMPRESSED_RGBA_ASTC_4x4_KHR(0x93B0, 8.00, '4x4'),
  COMPRESSED_RGBA_ASTC_5x4_KHR(0x93B1, 6.40, '5x4'),
  COMPRESSED_RGBA_ASTC_5x5_KHR(0x93B2, 5.12, '5x5'),
  COMPRESSED_RGBA_ASTC_6x5_KHR(0x93B3, 4.27, '6x5'),
  COMPRESSED_RGBA_ASTC_6x6_KHR(0x93B4, 3.56, '6x6'),
  COMPRESSED_RGBA_ASTC_8x5_KHR(0x93B5, 3.20, '8x5'),
  COMPRESSED_RGBA_ASTC_8x6_KHR(0x93B6, 2.67, '8x6'),
  COMPRESSED_RGBA_ASTC_8x8_KHR(0x93B7, 2.00, '8x8'),
  COMPRESSED_RGBA_ASTC_10x5_KHR(0x93B8, 2.56, '10x5'),
  COMPRESSED_RGBA_ASTC_10x6_KHR(0x93B9, 2.13, '10x6'),
  COMPRESSED_RGBA_ASTC_10x8_KHR(0x93BA, 1.60, '10x8'),
  COMPRESSED_RGBA_ASTC_10x10_KHR(0x93BB, 1.28, '10x10'),
  COMPRESSED_RGBA_ASTC_12x10_KHR(0x93BC, 1.07, '12x10'),
  COMPRESSED_RGBA_ASTC_12x12_KHR(0x93BD, 0.89, '12x12');

  final int value;
  final double bitsPerPixel;
  final String cmdLineName;

  const ASTCQuality(this.value, this.bitsPerPixel, this.cmdLineName);
}

class TextureService {
  static const _cjxlExe = '/Users/will/RiderProjects/NebulaCore/nebula_editor/deps/osx-arm64/cjxl';
  static const _djxlExe = '/Users/will/RiderProjects/NebulaCore/nebula_editor/deps/osx-arm64/djxl';
  static const _astcencExe = '/Users/will/RiderProjects/NebulaCore/nebula_editor/deps/osx-arm64/astcenc';

  static Future<void> losslessCompressTexture(String inPath, String outPath) async => await Process.run(_cjxlExe, [inPath, outPath, '-d 0.0']);
  static Future<void> losslessDecompressTexture(String inPath, String outPath) async => await Process.run(_djxlExe, [inPath, outPath]);
  static Future<void> lossyBundleTexture(String inPath, String outPath, ASTCQuality quality, bool srgb) async => await Process.run(_astcencExe, [srgb ? '-cs' : '-cl', inPath, outPath, quality.cmdLineName]);

  static ASTCQuality estimateAstcQuality(int width, int height, int targetBits) {
    var pixels = width * height;

    var lowestDistance = double.infinity;
    var lowestDistanceQuality = ASTCQuality.COMPRESSED_RGBA_ASTC_4x4_KHR;
    for (var quality in ASTCQuality.values) {
      var bits = pixels * quality.bitsPerPixel;
      var distance = (targetBits - bits).abs();
      if (distance < lowestDistance) {
        lowestDistance = distance;
        lowestDistanceQuality = quality;
      }
    }

    return lowestDistanceQuality;
  }
}
