import { HttpInterceptorFn } from '@angular/common/http';

interface Bearer {
  Name: string;
  Authorization: string;
}

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const token: Bearer = JSON.parse(localStorage.getItem('bearer') ?? null!);
  let newReq = req;
  if (token != null) {
    newReq = req.clone({
      headers: req.headers.set('Authorization', token.Authorization.toString()),
    });
  }
  return next(newReq);
};
