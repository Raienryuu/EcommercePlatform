import { ImageLoaderConfig } from "@angular/common"
import { environment } from "src/enviroment"

export const imageLoader = (config: ImageLoaderConfig) => {
  const imageNumber = config.src.split('-')[2];
  let url = environment.tempImagesUrl + `v1/image?`;

  if (config.loaderParams!['id'] != null) {
    url += 'productId=' + config.loaderParams!['id'].split('-')[1];
  } else {
    throw new Error("id have to be supplied")
  }
  if (config.width)
    url += '&width=' + config.width;
  if (imageNumber) {
    url += '&imageNumber=' + imageNumber;
  }
  return url;
}

