import { ImageLoaderConfig } from "@angular/common"
import { environment } from "src/enviroment"

export const imageLoader = (config: ImageLoaderConfig) => {
  let url = environment.apiUrl + `image?id=${config.src}&width=${config.width}`;
  const loader = config.loaderParams;
  if (!loader) return url;
  if (loader['number']) {
    url += `&number=${loader['number']}`;
  }
  return url;
}
