import { HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { saveAs } from 'file-saver';

@Injectable({
  providedIn: 'root'
})
export class FileService {

  downloadResponseContent(response: HttpResponse<Blob>): void {
    if (!response.body) {
      return;
    }
    const fileName = response.headers.get('content-disposition')?.split(';')[1].split('=')[1]!;
    this.download(response.body, fileName);
  }

  private download(blob: Blob, fileName: string): void {
    if (!blob) {
      return;
    }

    const file = new Blob([blob], { type: blob.type });
    saveAs(file, decodeURI(fileName || 'export'));
  }
}
