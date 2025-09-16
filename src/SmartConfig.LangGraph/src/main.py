#!/usr/bin/env python3
"""
Main entry point for the LangGraph Content Creation Supervisor.
"""

import os
import uuid
from dotenv import load_dotenv
from .supervisor_graph import graph

# Load environment variables
load_dotenv()


# Creates content using the LLM supervisor workflow
def create_content(prompt: str) -> dict:
    """
    Create content using the LLM supervisor workflow.
    
    Args:
        prompt: The user's content creation request
        
    Returns:
        dict: The final state with the created content
    """
    
    # Create a completely fresh state with unique identifier
    session_id = str(uuid.uuid4())[:8]  # Short unique ID for this session
    
    initial_state = {
        "messages": [
            {
                "role": "user",
                "content": prompt
            }
        ],
        "current_stage": "",
        "completed_stages": [],
        "session_id": session_id  # Add unique session ID
    }
    
    print(f"🚀 Starting LLM Supervisor Content Creation Workflow (Session: {session_id})")
    print("=" * 60)
    
    # Run the workflow with fresh state
    result = graph.invoke(initial_state)
    
    print(f"\n🎉 Workflow completed successfully! (Session: {session_id})")
    print(f"📊 Total messages processed: {len(result['messages'])}")
    print(f"✅ Stages completed: {result.get('completed_stages', [])}")
    
    return result


# Main CLI function for interactive content creation
def main():
    """Main function for command-line usage."""
    
    print("🤖 LangGraph Content Creation Supervisor")
    print("=" * 50)
    
    # Get user input
    prompt = input("Enter your content creation request: ")
    
    if not prompt.strip():
        print("❌ No prompt provided. Exiting.")
        return
    
    try:
        # Create content with fresh state
        result = create_content(prompt)
        
        # Show final content
        print("\n📄 FINAL CONTENT:")
        print("=" * 60)
        
        final_message = result['messages'][-1]
        if hasattr(final_message, 'content'):
            final_content = final_message.content
            print(final_content)
            print(f"\n📝 Content length: {len(final_content)} characters")
        else:
            print("No final content found.")
            
    except Exception as e:
        print(f"❌ Error during content creation: {e}")
        import traceback
        traceback.print_exc()


if __name__ == "__main__":
    main()


