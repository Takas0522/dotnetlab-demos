import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Category, CreateCategory, UpdateCategory } from '../models/category.model';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {
  private readonly apiUrl = '/api/categories';

  constructor(private http: HttpClient) {}

  getCategoriesByUserId(userId: number): Observable<Category[]> {
    return this.http.get<Category[]>(`${this.apiUrl}/user/${userId}`);
  }

  getCategoryById(id: number, userId: number): Observable<Category> {
    return this.http.get<Category>(`${this.apiUrl}/${id}/user/${userId}`);
  }

  createCategory(category: CreateCategory): Observable<Category> {
    return this.http.post<Category>(this.apiUrl, category);
  }

  updateCategory(id: number, userId: number, category: UpdateCategory): Observable<Category> {
    return this.http.put<Category>(`${this.apiUrl}/${id}/user/${userId}`, category);
  }

  deleteCategory(id: number, userId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}/user/${userId}`);
  }
}
