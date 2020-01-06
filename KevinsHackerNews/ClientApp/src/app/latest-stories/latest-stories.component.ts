import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-latest-stories',
  templateUrl: './latest-stories.component.html'
})
export class LatestStoriesComponent {
  public stories: NewsStory[];
  private startIndex: number = 0;
  private pageSize: number = 10;
  public pageNumber: number = 0;
  private _baseUrl: string;
  private httpClient: HttpClient;

  private getStories() {
    this.httpClient.get<NewsStory[]>(this._baseUrl + 'api/hackernews/getlateststories/' + this.startIndex + '/' + this.pageSize).subscribe(result => {
      this.stories = result;
    }, error => console.error(error));
  }

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this._baseUrl = baseUrl;
    this.httpClient = http;
    this.getStories();
  }

  public nextPage() {
    this.pageNumber++;
    this.startIndex = this.pageNumber * this.pageSize;
    this.stories = null;
    this.getStories();
  }

  public previousPage() {
    if (this.pageNumber > 0) {
      this.pageNumber--;
      this.startIndex = this.pageNumber * this.pageSize;
      this.stories = null;
      this.getStories();
    }
  }

}

interface NewsStory {
  id: number;
  title: string;
  url: string;
  text: string;
  by: string;
  time: Date;
  descendants: number;
}
