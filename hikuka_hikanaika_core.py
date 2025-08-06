# hikuka_hikanaika_core.py
# HikukaHikanaikaゲームのコア機能を提供するモジュール
# OpenAI GPT-4を使用して、リアリティーショーのキャラクター対話を実現

import os
import json
import openai

class HikukaHikanaikaCore:
    """HikukaHikanaikaゲームのメインクラス
    
    3人の女性キャラクターとのリアリティーショー対話ゲームを管理します。
    OpenAI GPT-4を使用してキャラクターの応答を生成します。
    """
    
    def __init__(self):
        """コンストラクタ - OpenAIクライアントを初期化し、キャラクター設定を読み込み"""
        # 環境変数からOpenAI APIキーを取得してクライアントを初期化
        self.client = openai.OpenAI(api_key=os.environ.get("OPENAI_API_KEY"))
        
        # キャラクター設定を読み込み
        self.character_data = self._load_character_data()
        
        # リアリティーショーのシステムプロンプト
        self.REALITY_SHOW_PROMPT = """
        あなたは『HikukaHikanaika』リアリティーショーの司会者です。
        3人の美女が競い合うリアリティーショーの進行と解説を行います。
        
        司会者の特徴:
        - テレビ番組の司会者らしく、エンターテインメント性を重視
        - 視聴者を引き込む演出とコメント
        - 各キャラクターの美容ポイントを分析
        - 競争を煽る実況スタイル
        
        番組の流れ:
        1. キャラクターの紹介と美容レベルの発表
        2. 競争の結果分析
        3. 視聴者へのエンターテイメント提供
        
        盛り上がる演出で、リアリティーショーらしい解説をしてください。
        """

    def get_ai_response(self, messages):
        """OpenAI GPT-4に会話履歴を送信し、応答を取得
        
        Args:
            messages (list): 会話履歴のリスト
            
        Returns:
            str: AIの応答テキスト
        """
        response = self.client.chat.completions.create(
            model="gpt-4o",
            messages=messages
        )
        return response.choices[0].message.content

    def introduce_character(self, character_name, beauty_stats):
        """キャラクターの自己紹介を生成
        
        Args:
            character_name (str): キャラクター名
            beauty_stats (dict): 美容ステータス情報
            
        Returns:
            dict: キャラクターの自己紹介メッセージ
        """
        # キャラクター情報を取得
        character_info = self.character_data.get("characters", {}).get(character_name, {})
        
        # キャラクター専用のシステムプロンプトを構築
        character_prompt = f"""
        あなたは『HikukaHikanaika』リアリティーショーの参加者、{character_name}です。
        
        キャラクター設定:
        - 名前: {character_info.get('name', character_name)}
        - 性格: {character_info.get('personality', '明るく親しみやすい')}
        - 特徴: {character_info.get('traits', '美容に関心が高い')}
        - 口調: {character_info.get('speech_style', '親しみやすい話し方')}
        
        現在の美容ステータス:
        - ファッション: {beauty_stats.get('fashion', 'N/A')}
        - 全体的な印象: {beauty_stats.get('overall', 'N/A')}
        
        リアリティーショーの参加者として、自分の魅力をアピールする自己紹介をしてください。
        他の参加者に負けない個性と美容へのこだわりを表現してください。
        """
        
        messages = [
            {"role": "system", "content": character_prompt},
            {"role": "user", "content": "リアリティーショーの視聴者に向けて、魅力的な自己紹介をしてください！"}
        ]
        
        introduction = self.get_ai_response(messages)
        
        return {
            "character_name": character_name,
            "character_info": character_info,
            "beauty_stats": beauty_stats,
            "introduction": introduction
        }

    def explain_reality_show(self, contestants):
        """リアリティーショーの解説を生成
        
        Args:
            contestants (list): 参加者リスト（各参加者の情報を含む辞書のリスト）
            
        Returns:
            dict: リアリティーショーの解説メッセージ
        """
        # 参加者情報を整理
        contestant_summary = []
        for contestant in contestants:
            name = contestant.get('character_name', 'Unknown')
            stats = contestant.get('beauty_stats', {})
            contestant_summary.append(f"{name}: {stats}")
        
        contest_info = "\n".join(contestant_summary)
        
        messages = [
            {"role": "system", "content": self.REALITY_SHOW_PROMPT},
            {"role": "user", "content": f"""
            今回のリアリティーショー『HikukaHikanaika』の参加者と結果を解説してください。
            
            参加者情報:
            {contest_info}
            
            テレビ番組の司会者として、この結果を面白く、かつドラマチックに解説してください。
            各参加者の美容ポイントの分析と、競争の見どころを含めてください。
            """}
        ]
        
        explanation = self.get_ai_response(messages)
        
        return {
            "contestants": contestants,
            "explanation": explanation,
            "show_title": "HikukaHikanaika リアリティーショー"
        }

    def get_character_list(self):
        """利用可能なキャラクターのリストを取得
        
        Returns:
            list: キャラクター名のリスト
        """
        return list(self.character_data.get("characters", {}).keys())

    def get_character_info(self, character_name):
        """指定されたキャラクターの詳細情報を取得
        
        Args:
            character_name (str): キャラクター名
            
        Returns:
            dict: キャラクター情報
        """
        return self.character_data.get("characters", {}).get(character_name, {})

    def _load_character_data(self):
        """キャラクター設定をJSONファイルから読み込み
        
        Returns:
            dict: キャラクターデータ
        """
        try:
            # 現在のファイルのディレクトリを取得
            current_dir = os.path.dirname(os.path.abspath(__file__))
            json_path = os.path.join(current_dir, "character_data.json")
            
            with open(json_path, 'r', encoding='utf-8') as f:
                return json.load(f)
        except (FileNotFoundError, json.JSONDecodeError) as e:
            print(f"警告: キャラクターデータファイルを読み込めませんでした: {e}")
            return self._get_default_character_data()

    def _get_default_character_data(self):
        """デフォルトのキャラクターデータを返す（JSONファイルが読み込めない場合のフォールバック）
        
        Returns:
            dict: デフォルトのキャラクターデータ
        """
        return {
            "characters": {
                "美咲": {
                    "name": "美咲（みさき）",
                    "personality": "自信満々で競争心が強い",
                    "traits": "ファッション誌のモデル経験あり、トレンドに敏感",
                    "speech_style": "自信に満ちた口調、時々高飛車"
                },
                "花音": {
                    "name": "花音（かのん）",
                    "personality": "優しく控えめだが芯が強い",
                    "traits": "ナチュラル美容のスペシャリスト、手作りコスメが得意",
                    "speech_style": "丁寧で優しい口調"
                },
                "麗奈": {
                    "name": "麗奈（れいな）",
                    "personality": "クールで戦略的",
                    "traits": "美容業界での仕事経験豊富、科学的なアプローチを重視",
                    "speech_style": "冷静で論理的な話し方"
                }
            },
            "beauty_categories": [
                "オートクチュールのドレス",
                "制服しか勝たん",
                "量産型ガーリー", 
                "清潔感あるけど量販感",
                "サイズ合ってない"
            ]
        }