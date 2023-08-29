// ignore_for_file: constant_identifier_names

enum ASTCQualities {
  COMPRESSED_RGBA_ASTC_4x4_KHR(0x93B0, 8.00),
  COMPRESSED_RGBA_ASTC_5x4_KHR(0x93B1, 6.40),
  COMPRESSED_RGBA_ASTC_5x5_KHR(0x93B2, 5.12),
  COMPRESSED_RGBA_ASTC_6x5_KHR(0x93B3, 4.27),
  COMPRESSED_RGBA_ASTC_6x6_KHR(0x93B4, 3.56),
  COMPRESSED_RGBA_ASTC_8x5_KHR(0x93B5, 3.20),
  COMPRESSED_RGBA_ASTC_8x6_KHR(0x93B6, 2.67),
  COMPRESSED_RGBA_ASTC_8x8_KHR(0x93B7, 2.00),
  COMPRESSED_RGBA_ASTC_10x5_KHR(0x93B8, 2.56),
  COMPRESSED_RGBA_ASTC_10x6_KHR(0x93B9, 2.13),
  COMPRESSED_RGBA_ASTC_10x8_KHR(0x93BA, 1.60),
  COMPRESSED_RGBA_ASTC_10x10_KHR(0x93BB, 1.28),
  COMPRESSED_RGBA_ASTC_12x10_KHR(0x93BC, 1.07),
  COMPRESSED_RGBA_ASTC_12x12_KHR(0x93BD, 0.89);

  final int value;
  final double bitsPerPixel;

  const ASTCQualities(this.value, this.bitsPerPixel);
}

class ModelService {
  static ASTCQualities getCompressionQuality(int width, int height, int targetBits) {
    var pixels = width * height;

    var lowestDistance = double.infinity;
    var lowestDistanceQuality = ASTCQualities.COMPRESSED_RGBA_ASTC_4x4_KHR;
    for (var quality in ASTCQualities.values) {
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
