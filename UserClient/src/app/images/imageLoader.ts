import { ImageLoaderConfig } from '@angular/common';
import { environment } from 'src/enviroment';

export const imageLoader = (config: ImageLoaderConfig) => {
  const idStartIndex = config.src.indexOf('-');
  const idEndIndex = config.src.lastIndexOf('-');

  const imageNumber = config.src.slice(idEndIndex + 1);
  const productId = config.src.slice(idStartIndex + 1, idEndIndex);

  let url = environment.apiUrl + `v1/image?`;

  if (productId != null || productId !== '') {
    url += 'productId=' + productId;
  } else {
    throw new Error('id has to be supplied');
  }
  if (config.width) url += '&width=' + config.width;
  if (imageNumber) {
    url += '&imageNumber=' + imageNumber;
  }
  return url;
};
