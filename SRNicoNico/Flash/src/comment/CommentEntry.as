package comment {
	import flash.filters.BevelFilter;
	import flash.text.TextField;
	import flash.text.TextFieldAutoSize;
	import flash.text.TextFormat;
	
	
	public class CommentEntry extends TextField {
		
		
		//べベルフィルター
		private static const DEFAULT_FILTERS:Array = [new BevelFilter(1, 45, 0, 1, 0, 1, 2, 2, 1, 1, "outer")];
		
		//コメントナンバー
		public var no:uint;
		
		//コメント表示開始時間
		public var vpos:Number;
		
		//コメント表示終了時間
		public var vend:Number;
		
		//コメント
		public var content:String;
		
		//投稿者コメントか否か
		public var fork:Boolean;
		
		//コメントのコマンド
		public var command:CommentCommand;
		
		//コメントデコレーション
		public var mail:String;
		
		//NGスコア
		public var score:int;
		
		public function CommentEntry(no:uint, vpos:Number, mail:String, content:String, isMe:Boolean, score:int) {
		
			//値指定
			this.no = no;									//コメントナンバー
			this.content = content;							//コメント
			this.mail = mail;
			this.command = new CommentCommand(mail);		//コメントコマンド
			this.vpos = vpos;// - (command.pos == CommentCommand.PLACE_NAKA ? 1000 : 0);			//コメント表示開始時間
			this.vend = this.vpos + this.command.duration;	//コメント表示時間
			
			
			super();
			
			//テキストフィールドの設定
			this.defaultTextFormat = new TextFormat("Arial", command.size, command.color, true);
			
			this.alpha = 0.8;
			this.multiline = true;
			this.selectable = false;
			this.autoSize = TextFieldAutoSize.CENTER;
			this.filters = DEFAULT_FILTERS;
			this.text = content;
			
			if (isMe) {
				
				this.border = true;
				this.borderColor = 0xFFFF00;
			}
			
			this.score = score;
			
			
			CommentSizeCalculator.calcScale(this, this.numLines);
		}
		
		
		
		
		
		
		
	}
}