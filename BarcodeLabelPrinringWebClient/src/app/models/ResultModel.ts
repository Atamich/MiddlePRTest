export interface ResultModel<T = any> {
    isSuccess: boolean;
    errorMessage: string;
    content: T;
  }
  